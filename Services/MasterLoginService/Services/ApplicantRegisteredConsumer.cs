using MasterLoginService.Data;
using MasterLoginService.Messaging;
using MasterLoginService.Models;
using Microsoft.EntityFrameworkCore;
using static MasterLoginService.DtoModels.LoginRequestDto;
using ApplicantRegisteredEvent = MasterLoginService.DtoModels.LoginRequestDto.ApplicantRegisteredEvent;

namespace MasterLoginService.Services
{
    public class ApplicantRegisteredConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly ILogger<ApplicantRegisteredConsumer> _logger;
        private readonly RabbitMqConfiguration _rabbitMqConfig;

        public ApplicantRegisteredConsumer(
            IServiceProvider serviceProvider,
            IRabbitMqService rabbitMqService,
            ILogger<ApplicantRegisteredConsumer> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _rabbitMqService = rabbitMqService;
            _logger = logger;
            _rabbitMqConfig = RabbitMqConfiguration.FromConfiguration(configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting RabbitMQ consumer service...");

            // Wait for RabbitMQ connection
            int retryCount = 0;
            const int maxRetries = 5;

            while (!_rabbitMqService.IsConnected && retryCount < maxRetries && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"Attempting to connect to RabbitMQ (attempt {retryCount + 1}/{maxRetries})...");

                    if (await _rabbitMqService.ConnectAsync())
                    {
                        break;
                    }

                    retryCount++;
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to connect to RabbitMQ on attempt {retryCount + 1}");
                    retryCount++;
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            if (!_rabbitMqService.IsConnected)
            {
                _logger.LogError("Failed to connect to RabbitMQ after maximum retries");
                return;
            }

            _logger.LogInformation("Successfully connected to RabbitMQ");

            // Start consuming events
            _rabbitMqService.StartConsumingAsync<ApplicantRegisteredEvent>(
                _rabbitMqConfig.ApplicantRegisteredQueue,
                async (eventData) =>
                {
                    await HandleApplicantRegistered(eventData);
                });

            _rabbitMqService.StartConsumingAsync<UserTypeUpdatedEvent>(
                _rabbitMqConfig.UserTypeUpdatedQueue,
                async (eventData) =>
                {
                    await HandleUserTypeUpdated(eventData);
                });

            _logger.LogInformation("RabbitMQ consumer service started successfully");

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task HandleApplicantRegistered(ApplicantRegisteredEvent eventData)
        {
            _logger.LogInformation($"Processing applicant registered event for: {eventData.LoginId}");

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MasterLoginDbContext>();

            try
            {
                // Validate and normalize UserType
                var normalizedUserType = NormalizeUserType(eventData.UserType);

                // Check if user already exists
                var existingUser = await dbContext.MasterUsers
                    .FirstOrDefaultAsync(u => u.LoginId == eventData.LoginId);

                if (existingUser == null)
                {
                    // Create new user in master directory
                    var masterUser = new MasterUser
                    {
                        LoginId = eventData.LoginId,
                        Email = eventData.Email,
                        MobileNo = eventData.MobileNo,
                        UserType = normalizedUserType, // Store as string
                        Name = eventData.Name,
                        RegistrationType = eventData.RegistrationType,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow,
                        IsActive = true
                    };

                    dbContext.MasterUsers.Add(masterUser);
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Created master user record for {eventData.LoginId} with type {normalizedUserType}");
                }
                else
                {
                    // Update existing user
                    existingUser.Email = eventData.Email;
                    existingUser.MobileNo = eventData.MobileNo;
                    existingUser.UserType = normalizedUserType; // Update as string
                    existingUser.Name = eventData.Name;
                    existingUser.RegistrationType = eventData.RegistrationType;
                    existingUser.LastUpdated = DateTime.UtcNow;
                    existingUser.IsActive = true;

                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Updated master user record for {eventData.LoginId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing applicant registered event for {eventData.LoginId}");
            }
        }

        private async Task HandleUserTypeUpdated(UserTypeUpdatedEvent eventData)
        {
            _logger.LogInformation($"Processing user type updated event for: {eventData.LoginId}");

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MasterLoginDbContext>();

            try
            {
                var user = await dbContext.MasterUsers
                    .FirstOrDefaultAsync(u => u.LoginId == eventData.LoginId);

                if (user != null)
                {
                    // Validate and normalize UserType
                    var normalizedUserType = NormalizeUserType(eventData.UserType);
                    user.UserType = normalizedUserType;
                    user.LastUpdated = DateTime.UtcNow;

                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Updated user type for {eventData.LoginId} to {normalizedUserType}");
                }
                else
                {
                    _logger.LogWarning($"User {eventData.LoginId} not found in master directory");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing user type updated event for {eventData.LoginId}");
            }
        }

        /// <summary>
        /// Normalizes user type string to ensure consistency
        /// </summary>
        private string NormalizeUserType(string userType)
        {
            if (string.IsNullOrWhiteSpace(userType))
                return "Applicant";

            // Convert to proper case
            return userType.ToLower() switch
            {
                "officer" => "Officer",
                "applicant" => "Applicant",
                "revenue" => "Revenue",
                "enumerator" => "Enumerator",
                "1" => "Officer",
                "2" => "Applicant",
                "3" => "Revenue",
                "4" => "Enumerator",
                _ => "Applicant" // Default fallback
            };
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RabbitMQ consumer service...");

            try
            {
                await _rabbitMqService.DisconnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RabbitMQ service shutdown");
            }

            await base.StopAsync(cancellationToken);
        }
    }
}


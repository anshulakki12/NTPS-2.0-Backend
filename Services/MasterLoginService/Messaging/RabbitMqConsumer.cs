// Services/MasterUserEventConsumer.cs
using MasterLoginService.Config;
using MasterLoginService.Data;
using MasterLoginService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MasterLoginService.Messaging
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMqConfig _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMqConsumer> _logger;

        public RabbitMqConsumer(
            IOptions<RabbitMqConfig> config,
            IServiceProvider serviceProvider,
            ILogger<RabbitMqConsumer> logger)
        {
            _config = config.Value;
            _serviceProvider = serviceProvider;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = _config.HostName,
                UserName = _config.UserName,
                Password = _config.Password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchange and queue (same as publisher)
            _channel.ExchangeDeclare(
                exchange: _config.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            _channel.QueueDeclare(
                queue: _config.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(
                queue: _config.QueueName,
                exchange: _config.ExchangeName,
                routingKey: _config.RoutingKey);

            _logger.LogInformation("MasterLoginService RabbitMQ consumer started");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                await ProcessMessageAsync(ea);
            };

            _channel.BasicConsume(
                queue: _config.QueueName,
                autoAck: false,
                consumer: consumer);

            await Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(BasicDeliverEventArgs ea)
        {
            string body = string.Empty;
            try
            {
                body = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogDebug("Received message: {Body}", body);

                var eventData = JsonSerializer.Deserialize<ApplicantRegisteredEvent>(body);
                if (eventData != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider
                            .GetRequiredService<MasterLoginDbContext>();

                        // Check if user already exists
                        var existingUser = await dbContext.MasterUsers
                            .FirstOrDefaultAsync(u => u.LoginId == eventData.LoginId);

                        if (existingUser == null)
                        {
                            // Create new MasterUser
                            var masterUser = new MasterUser
                            {
                                LoginId = eventData.LoginId,
                                MobileNo = eventData.MobileNo,
                                Email = eventData.Email,
                                Name = eventData.Name,
                                UserType = eventData.UserType,
                                RegistrationType = eventData.RegistrationType,
                                IsVerified = eventData.IsVerified,
                                CreatedDate = eventData.CreatedDate,
                                LastUpdated = DateTime.UtcNow
                            };

                            dbContext.MasterUsers.Add(masterUser);
                            await dbContext.SaveChangesAsync();

                            _logger.LogInformation(
                                "Created MasterUser for LoginId: {LoginId}",
                                eventData.LoginId);
                        }
                        else
                        {
                            // Update existing user
                            existingUser.Email = eventData.Email;
                            existingUser.MobileNo = eventData.MobileNo;
                            existingUser.Name = eventData.Name;
                            existingUser.IsVerified = eventData.IsVerified;
                            existingUser.LastUpdated = DateTime.UtcNow;

                            dbContext.MasterUsers.Update(existingUser);
                            await dbContext.SaveChangesAsync();

                            _logger.LogInformation(
                                "Updated MasterUser for LoginId: {LoginId}",
                                eventData.LoginId);
                        }
                    }

                    // Acknowledge message
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to deserialize message: {Body}", body);
                // Reject message - don't requeue
                _channel.BasicReject(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RabbitMQ message");
                // Reject and requeue for retry
                _channel.BasicReject(ea.DeliveryTag, true);
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }

    // Event model for MasterLoginService
    public class ApplicantRegisteredEvent
    {
        public string LoginId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RegistrationType { get; set; } = string.Empty;
        public string UserType { get; set; } = "Applicant";
        public DateTime CreatedDate { get; set; }
        public char IsVerified { get; set; } = 'N';
    }
}

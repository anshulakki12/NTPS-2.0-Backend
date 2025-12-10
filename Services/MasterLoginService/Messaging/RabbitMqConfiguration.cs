namespace MasterLoginService.Messaging
{
    public class RabbitMqConfiguration
    {
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";

        // Queue names
        public string ApplicantRegisteredQueue { get; set; } = "applicant.registered";
        public string UserTypeUpdatedQueue { get; set; } = "user.type.updated";

        // Exchange names (if using exchanges)
        public string UserEventsExchange { get; set; } = "user.events";

        // Retry configuration
        public int MaxRetryAttempts { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 5;

        public static RabbitMqConfiguration FromConfiguration(IConfiguration configuration)
        {
            return new RabbitMqConfiguration
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest",
                Port = int.TryParse(configuration["RabbitMQ:Port"], out var port) ? port : 5672,
                VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/",
                ApplicantRegisteredQueue = configuration["RabbitMQ:Queues:ApplicantRegistered"] ?? "applicant.registered",
                UserTypeUpdatedQueue = configuration["RabbitMQ:Queues:UserTypeUpdated"] ?? "user.type.updated"
            };
        }
    }
}


namespace MasterLoginService.Config
{
    public class RabbitMqConfig
    {
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string QueueName { get; set; } = "user.registration.queue";
        public string ExchangeName { get; set; } = "user.events";
        public string RoutingKey { get; set; } = "user.registered";
    }
}

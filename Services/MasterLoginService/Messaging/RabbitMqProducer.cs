using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MasterLoginService.Messaging
{
    public class RabbitMqProducer
    {
        private readonly IConfiguration _config;

        public RabbitMqProducer(IConfiguration config)
        {
            _config = config;
        }

        public Task PublishLoginEventAsync(object message)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_config["RabbitMQ:ConnectionString"])
            };

            // Create synchronous connection
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "masterlogin.events",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: "",
                routingKey: "masterlogin.events",
                basicProperties: null,
                body: body
            );

            return Task.CompletedTask;
        }
    }
}

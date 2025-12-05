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

        public async Task PublishLoginEventAsync(object message)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_config["RabbitMQ:ConnectionString"])
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "masterlogin.events",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "masterlogin.events",
                mandatory: false,
                body: body
            );
        }
    }
}

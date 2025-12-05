using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MasterLoginService.Messaging
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IConfiguration _config;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqConsumer(IConfiguration config)
        {
            _config = config;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_config["RabbitMQ:ConnectionString"])
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(null, cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: "masterlogin.events",
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            await base.StartAsync(cancellationToken);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
                throw new Exception("RabbitMQ channel not initialized.");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                string body = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine("Received Login event: " + body);

                await Task.Yield(); // required for async handler
            };

            await _channel.BasicConsumeAsync(
                queue: "masterlogin.events",
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken
            );
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null) await _channel.CloseAsync(cancellationToken);
            if (_connection != null) await _connection.CloseAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}

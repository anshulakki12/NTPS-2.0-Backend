using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MasterLoginService.Messaging
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqService> _logger;
        private IConnection _connection;
        private IModel _channel;
        private bool _disposed;

        public bool IsConnected => _connection?.IsOpen == true;

        public RabbitMqService(
            IConfiguration configuration,
            ILogger<RabbitMqService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Establishes connection to RabbitMQ server
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            try
            {
                if (IsConnected)
                {
                    _logger.LogInformation("Already connected to RabbitMQ");
                    return true;
                }

                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                    UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                    Password = _configuration["RabbitMQ:Password"] ?? "guest",
                    Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                    DispatchConsumersAsync = true, // Enable async consumers
                    AutomaticRecoveryEnabled = true, // Enable automatic reconnection
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10) // Try to reconnect every 10 seconds
                };

                // Add virtual host if configured
                if (!string.IsNullOrEmpty(_configuration["RabbitMQ:VirtualHost"]))
                {
                    factory.VirtualHost = _configuration["RabbitMQ:VirtualHost"];
                }

                _logger.LogInformation($"Connecting to RabbitMQ at {factory.HostName}:{factory.Port}...");

                _connection = await Task.Run(() => factory.CreateConnection());
                _channel = _connection.CreateModel();

                // Configure channel
                _channel.BasicQos(0, 1, false); // Fair dispatch - one message at a time

                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;
                _connection.ConnectionUnblocked += OnConnectionUnblocked;

                _logger.LogInformation("Successfully connected to RabbitMQ");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ");
                return false;
            }
        }

        /// <summary>
        /// Publishes a message to the specified queue
        /// </summary>
        public void Publish<T>(string queueName, T message)
        {
            if (!IsConnected)
            {
                _logger.LogWarning("Cannot publish message: RabbitMQ connection is not established");
                throw new InvalidOperationException("RabbitMQ connection is not established");
            }

            try
            {
                // Ensure queue exists
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true, // Messages survive broker restart
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true; // Message survives broker restart
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: queueName,
                    basicProperties: properties,
                    body: body);

                _logger.LogDebug($"Published message to queue '{queueName}': {json}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to publish message to queue '{queueName}'");
                throw;
            }
        }

        /// <summary>
        /// Starts consuming messages from the specified queue
        /// </summary>
        public void StartConsuming<T>(string queueName, Action<T> handler)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("RabbitMQ connection is not established");
            }

            try
            {
                // Ensure queue exists
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);

                        _logger.LogDebug($"Received message from queue '{queueName}': {json}");

                        var message = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (message != null)
                        {
                            handler(message);
                        }

                        // Acknowledge message
                        _channel.BasicAck(ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing message from queue '{queueName}'");

                        // Negative acknowledgement - requeue message
                        _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                // Start consuming
                _channel.BasicConsume(
                    queue: queueName,
                    autoAck: false, // Manual acknowledgment
                    consumer: consumer);

                _logger.LogInformation($"Started consuming messages from queue '{queueName}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to start consuming from queue '{queueName}'");
                throw;
            }
        }

        /// <summary>
        /// Starts consuming messages from the specified queue asynchronously
        /// </summary>
        public void StartConsumingAsync<T>(string queueName, Func<T, Task> handler)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("RabbitMQ connection is not established");
            }

            try
            {
                // Ensure queue exists
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);

                        _logger.LogDebug($"Received message from queue '{queueName}': {json}");

                        var message = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (message != null)
                        {
                            await handler(message);
                        }

                        // Acknowledge message
                        _channel.BasicAck(ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing message from queue '{queueName}'");

                        // Negative acknowledgement - requeue message
                        _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                // Start consuming
                _channel.BasicConsume(
                    queue: queueName,
                    autoAck: false, // Manual acknowledgment
                    consumer: consumer);

                _logger.LogInformation($"Started consuming messages asynchronously from queue '{queueName}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to start consuming from queue '{queueName}'");
                throw;
            }
        }

        /// <summary>
        /// Disconnects from RabbitMQ server
        /// </summary>
        public async Task DisconnectAsync()
        {
            try
            {
                if (_channel?.IsOpen == true)
                {
                    _channel.Close();
                    _channel.Dispose();
                    _channel = null;
                }

                if (_connection?.IsOpen == true)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }

                _logger.LogInformation("Disconnected from RabbitMQ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disconnecting from RabbitMQ");
                throw;
            }
        }

        #region Event Handlers

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogWarning($"RabbitMQ connection shut down: {e.ReplyText}");
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            _logger.LogError(e.Exception, "RabbitMQ callback exception occurred");
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            _logger.LogWarning($"RabbitMQ connection blocked: {e.Reason}");
        }

        private void OnConnectionUnblocked(object sender, EventArgs e)
        {
            _logger.LogInformation("RabbitMQ connection unblocked");
        }

        #endregion

        #region IDisposable Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        DisconnectAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error during RabbitMQ service disposal");
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~RabbitMqService()
        {
            Dispose(disposing: false);
        }

        #endregion
    }
}


namespace MasterLoginService.Messaging
{
    public interface IRabbitMqService : IDisposable
    {
        /// <summary>
        /// Publishes a message to the specified RabbitMQ queue
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="queueName">Queue name</param>
        /// <param name="message">Message to publish</param>
        void Publish<T>(string queueName, T message);

        /// <summary>
        /// Starts consuming messages from the specified queue
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="queueName">Queue name</param>
        /// <param name="handler">Handler function for incoming messages</param>
        void StartConsuming<T>(string queueName, Action<T> handler);

        /// <summary>
        /// Starts consuming messages from the specified queue asynchronously
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="queueName">Queue name</param>
        /// <param name="handler">Async handler function for incoming messages</param>
        void StartConsumingAsync<T>(string queueName, Func<T, Task> handler);

        /// <summary>
        /// Checks if RabbitMQ connection is established
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connects to RabbitMQ server
        /// </summary>
        Task<bool> ConnectAsync();

        /// <summary>
        /// Disconnects from RabbitMQ server
        /// </summary>
        Task DisconnectAsync();
    }
}

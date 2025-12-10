namespace ApplicantAuthenticationService.Services
{
    public interface IRabbitMqPublisher
    {
        void Publish<T>(string queueName, T message);
        void StartConsuming<T>(string queueName, Action<T> handler);
    }
}

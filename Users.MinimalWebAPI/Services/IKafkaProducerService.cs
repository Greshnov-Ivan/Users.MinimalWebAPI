namespace Users.MinimalWebAPI.Services
{
    public interface IKafkaProducerService
    {
        public Task SendMessageAsync<T>(string topic, T message, CancellationToken cancellationToken);
    }
}

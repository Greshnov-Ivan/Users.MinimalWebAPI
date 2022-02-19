using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Users.MinimalWebAPI.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(ILogger<KafkaProducerService> logger, IOptions<KafkaProducerConfig> options)
        {
            _logger = logger;
            // Укажем где находятся серверы начальной загрузки
            var config = new ProducerConfig()
            {
                BootstrapServers = options.Value.BootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }
        public async Task SendMessageAsync<T>(string topic, T message, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.ProduceAsync(topic, new Message<Null, string> { Value = JsonSerializer.Serialize<T>(message) }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Oops, something went wrong: {ex}");
            }
        }
    }
}

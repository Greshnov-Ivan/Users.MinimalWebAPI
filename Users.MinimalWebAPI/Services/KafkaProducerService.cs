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
            Task sendMessageToKafka = Task.Run(() => _producer.ProduceAsync(topic, new Message<Null, string> { Value = JsonSerializer.Serialize<T>(message) }, cancellationToken));
            await WithTimeout(sendMessageToKafka, TimeSpan.FromSeconds(10));
        }
        private async Task WithTimeout(Task task, TimeSpan time)
        {
            Task delayTask = Task.Delay(time);
            Task firstToFinish = await Task.WhenAny(task, delayTask);

            if (firstToFinish == delayTask)
            {
                // Первой закончилась задача задержки - разберёмся с исключением
                task.ContinueWith(HandleException);
                throw new TimeoutException();
            }

            await task;
        }

        private void HandleException(Task task)
        {
            if (task.Exception is not null)
            {
                _logger.LogError($"Oops, something went wrong: {task.Exception.Message}");
            }
        }
    }
}

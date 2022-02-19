namespace Users.MinimalWebAPI
{
    public class KafkaProducerConfig
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public TopicsConfig? Topics { get; set; }
    }
}
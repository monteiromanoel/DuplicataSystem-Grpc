namespace Duplicata.Infrastructure.Kafka
{
    public class KafkaSettings
    {
        public const string SectionName = "Kafka";
        public string BootstrapServers { get; set; } = "localhost:29092";
    }
}

using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace Duplicata.Infrastructure.Kafka
{
    /// <summary>
    /// Garante que os tópicos do Kafka existam antes dos consumers iniciarem.
    /// Evita erro "Subscribed topic not available: Unknown topic or partition".
    /// </summary>
    public static class KafkaTopicEnsurer
    {
        public static readonly string[] DefaultTopics =
        {
            "duplicata.created",
            "duplicata.registered",
            "duplicata.paid",
            "duplicata.rejected"
        };

        /// <summary>
        /// Cria os tópicos se não existirem. Ignora TopicAlreadyExists.
        /// </summary>
        public static async Task EnsureTopicsAsync(string bootstrapServers, string[]? topics = null)
        {
            topics ??= DefaultTopics;
            using var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = bootstrapServers
            }).Build();

            var specifications = topics.Select(t => new TopicSpecification
            {
                Name = t,
                NumPartitions = 1,
                ReplicationFactor = 1
            }).ToList();

            try
            {
                await adminClient.CreateTopicsAsync(specifications, new CreateTopicsOptions { RequestTimeout = TimeSpan.FromSeconds(10) });
            }
            catch (CreateTopicsException ex)
            {
                foreach (var result in ex.Results)
                {
                    if (result.Error.Code != ErrorCode.TopicAlreadyExists)
                    {
                        throw new InvalidOperationException($"Falha ao criar tópico '{result.Topic}': {result.Error.Reason}", ex);
                    }
                }
            }
        }
    }
}

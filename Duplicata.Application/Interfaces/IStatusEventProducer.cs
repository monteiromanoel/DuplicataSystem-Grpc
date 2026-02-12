namespace Duplicata.Application.Interfaces
{
    /// <summary>
    /// Abstração para publicar eventos de status de duplicata (simulando respostas externas como B3).
    /// Permite implementação real (Kafka/B3) ou mock para testes.
    /// </summary>
    public interface IStatusEventProducer
    {
        Task PublishRegisteredAsync(Guid duplicataId);
        Task PublishPaidAsync(Guid duplicataId);
        Task PublishRejectedAsync(Guid duplicataId);
    }
}

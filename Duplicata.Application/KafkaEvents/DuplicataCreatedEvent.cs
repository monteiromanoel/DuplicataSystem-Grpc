namespace Duplicata.Application.KafkaEvents
{
    public record DuplicataCreatedEvent(
        Guid Id,
        string Numero,
        decimal Valor,
        DateTime Vencimento
    );
}

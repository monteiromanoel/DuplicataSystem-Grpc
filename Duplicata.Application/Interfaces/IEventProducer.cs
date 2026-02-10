namespace Duplicata.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync(string topic, object message);
    }
}

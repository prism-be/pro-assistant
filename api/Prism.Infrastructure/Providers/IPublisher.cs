namespace Prism.Infrastructure.Providers;

public interface IPublisher
{
    Task PublishAsync<T>(string queue, T message);
}
namespace Prism.ProAssistant.Domain;

public interface IHydrator
{
    Task<T?> Hydrate<T>(string? streamId);
}
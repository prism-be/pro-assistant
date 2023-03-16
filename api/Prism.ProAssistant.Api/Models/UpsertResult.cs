using MongoDB.Driver;

namespace Prism.ProAssistant.Api.Models;

public record UpsertResult(string? Id)
{
    public static UpsertResult FromResult(UpdateResult result)
    {
        return new UpsertResult(result.UpsertedId.ToString());
    }
}
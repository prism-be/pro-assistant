using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Storage;

namespace Prism.ProAssistant.Api.Controllers.Data;

public interface IDataController<T>
{
    Task<UpsertResult> Insert([FromBody] T request);
    Task<List<T>> List();
    Task<List<T>> Search([FromBody] List<SearchFilter> request);
    Task<T?> Single(string id);
    Task<UpsertResult> Update([FromBody] T request);
}
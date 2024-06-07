using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Controllers.Data;

using Core;
using Domain;
using Helpers;

[Authorize]
public class DocumentConfigurationController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly IQueryService _queryService;

    public DocumentConfigurationController(IQueryService queryService, IEventStore eventStore)
    {
        _queryService = queryService;
        _eventStore = eventStore;
    }

    [HttpDelete]
    [Route("api/data/document-configurations/{id}")]
    public async Task<UpsertResult> Delete(string id)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _eventStore.RaiseAndPersist<DocumentConfiguration>(new DocumentConfigurationDeleted { StreamId = id });
    }

    [HttpPost]
    [Route("api/data/document-configurations/insert")]
    public async Task<UpsertResult> Insert([FromBody] DocumentConfiguration request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        request.Id = Identifier.GenerateString();
        
        return await _eventStore.RaiseAndPersist<DocumentConfiguration>(new DocumentConfigurationCreated { DocumentConfiguration = request });
    }

    [HttpGet]
    [Route("api/data/document-configurations")]
    public async Task<IEnumerable<DocumentConfiguration>> List()
    {
        return await _queryService.ListAsync<DocumentConfiguration>();
    }

    [HttpPost]
    [Route("api/data/document-configurations/search")]
    public async Task<IEnumerable<DocumentConfiguration>> Search([FromBody] IEnumerable<Filter> request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _queryService.SearchAsync<DocumentConfiguration>(request.ToArray());
    }

    [HttpGet]
    [Route("api/data/document-configurations/{id}")]
    public async Task<DocumentConfiguration?> Single(string id)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _queryService.SingleOrDefaultAsync<DocumentConfiguration>(id);
    }

    [HttpPost]
    [Route("api/data/document-configurations/update")]
    public async Task<UpsertResult> Update([FromBody] DocumentConfiguration request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _eventStore.RaiseAndPersist<DocumentConfiguration>(new DocumentConfigurationUpdated { DocumentConfiguration = request });
    }
}
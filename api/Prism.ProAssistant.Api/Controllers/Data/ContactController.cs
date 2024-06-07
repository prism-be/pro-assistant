namespace Prism.ProAssistant.Api.Controllers.Data;

using Core;
using Domain;
using Domain.DayToDay.Contacts;
using Domain.DayToDay.Contacts.Events;
using Helpers;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage;
using Storage.Events;

[Authorize]
public class ContactController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly IQueryService _queryService;

    public ContactController(IQueryService queryService, IEventStore eventStore)
    {
        _eventStore = eventStore;
        _queryService = queryService;
    }

    [HttpPost]
    [Route("api/data/contacts/insert")]
    public async Task<UpsertResult> Insert([FromBody] Contact request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        request.Id = Identifier.GenerateString();

        return await _eventStore.RaiseAndPersist<Contact>(new ContactCreated
        {
            Contact = request
        });
    }

    [HttpGet]
    [Route("api/data/contacts")]
    public async Task<IEnumerable<Contact>> List()
    {
        return await _queryService.ListAsync<Contact>();
    }

    [HttpPost]
    [Route("api/data/contacts/search")]
    public async Task<IEnumerable<Contact>> Search([FromBody] Filter[] request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _queryService.SearchAsync<Contact>(request);
    }

    [HttpGet]
    [Route("api/data/contacts/{id}")]
    public async Task<Contact?> Single(string id)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _queryService.SingleAsync<Contact>(id);
    }

    [HttpPost]
    [Route("api/data/contacts/update")]
    public async Task<UpsertResult> Update([FromBody] Contact request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        var result = await _eventStore.RaiseAndPersist<Contact>(new ContactUpdated
        {
            Contact = request
        });

        return result;
    }
}
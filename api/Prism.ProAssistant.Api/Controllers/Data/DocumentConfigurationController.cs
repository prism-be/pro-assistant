using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class DocumentConfigurationController : Controller
{
    private readonly IDataService _dataService;

    public DocumentConfigurationController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpPost]
    [Route("api/data/document-configurations/insert")]
    public async Task<UpsertResult> Insert([FromBody] DocumentConfiguration request)
    {
        return await _dataService.InsertAsync(request);
    }

    [HttpGet]
    [Route("api/data/document-configurations")]
    public async Task<List<DocumentConfiguration>> List()
    {
        return await _dataService.ListAsync<DocumentConfiguration>();
    }

    [HttpPost]
    [Route("api/data/document-configurations/search")]
    public async Task<List<DocumentConfiguration>> Search([FromBody] List<SearchFilter> request)
    {
        return await _dataService.SearchAsync<DocumentConfiguration>(request);
    }

    [HttpPost]
    [Route("api/data/document-configurations/update")]
    public async Task<UpsertResult> Update([FromBody] DocumentConfiguration request)
    {
        return await _dataService.UpdateAsync(request);
    }

    [HttpGet]
    [Route("api/data/document-configurations/{id}")]
    public async Task<DocumentConfiguration?> Single(string id)
    {
        return await _dataService.SingleAsync<DocumentConfiguration>(id);
    }
}
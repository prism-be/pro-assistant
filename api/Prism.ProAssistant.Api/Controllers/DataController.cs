using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers;

public class DataController : Controller
{
    private readonly IUserOrganizationService _userOrganizationService;

    public DataController(IUserOrganizationService userOrganizationService)
    {
        _userOrganizationService = userOrganizationService;
    }

    [HttpPost]
    [Route("api/data/{collectionName}/search")]
    public ActionResult Search(string collectionName, [FromBody] dynamic request)
    {
        var collection = _userOrganizationService.GetUserCollection(collectionName);
        return Ok(new List<string>());
    }
}
using Microsoft.AspNetCore.Mvc;

namespace Prism.ProAssistant.Api.Controllers;

public class DataController : Controller
{
    [HttpPost]
    [Route("api/data/{collection}/search")]
    public ActionResult Search(string collection, [FromBody] dynamic request)
    {
        return Ok(new List<string>());
    }
}
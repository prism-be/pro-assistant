using Microsoft.AspNetCore.Mvc;

namespace Prism.ProAssistant.Api.Controllers;

public class DataController: Controller
{
    [HttpPost]
    [Route("api/data/{collection}/search")]
    public ActionResult Search(string collection, [FromBody] dynamic request)
    {
        return this.Ok(new List<string>());
    }
}
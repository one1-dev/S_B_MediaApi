using Microsoft.AspNetCore.Mvc;
using S_B_MediaApi.Models;


namespace S_B_MediaApi.Controllers;


[ApiController]
[Route("resources")]
public class ResourcesController : ControllerBase
{
[HttpGet]
public ActionResult<ResourceList> Get() => Ok(new ResourceList(new[]
{
"contracts", "consumers", "invoices", "profiles", "templates", "jobs", "batchRequest"
}));
}
using Microsoft.AspNetCore.Mvc;

namespace S_B_MediaApi.Controllers;

[ApiController]
[Route("jobs")] 
public class JobsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<object>> GetJobs() => Ok(new[] { new { id = 12345, type = "invoice", status = "running" } });

    [HttpGet("{id:int}")]
    public ActionResult<object> GetJob(int id) => Ok(new { id, status = "ok" });
}
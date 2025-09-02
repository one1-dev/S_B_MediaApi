using Microsoft.AspNetCore.Mvc;
using S_B_MediaApi.Models;

namespace S_B_MediaApi.Controllers;

[ApiController]
[Route("profiles")]
public class ProfilesController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<UsageProfile>> Get() => Ok(new[]
    {
        new UsageProfile{ Id = 1, Name = "Standard", Description = "Default", Href = "/profiles/1/detail" }
    });

    [HttpGet("{id:int}/detail")]
    public ActionResult<UsageProfile> GetDetail(int id) => Ok(new UsageProfile{ Id = id, Name = "Standard" });
}
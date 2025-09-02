using Microsoft.AspNetCore.Mvc;
using S_B_MediaApi.Models;


namespace S_B_MediaApi.Controllers;


[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
[HttpGet]
public ActionResult<VersionEnvelope> Get() => Ok(new VersionEnvelope("1.11.0 (CustomerAdministrationService: 6.18.5)"));
}
using Microsoft.AspNetCore.Mvc;

namespace S_B_MediaApi.Controllers;

[ApiController]
[Route("templates")] 
public class TemplatesController : ControllerBase
{
    [HttpGet("contracts")]
    public ActionResult<IEnumerable<string>> GetContractTemplates() => Ok(new[] { "DefaultContract" });

    [HttpGet("consumers")]
    public ActionResult<IEnumerable<string>> GetConsumerTemplates() => Ok(new[] { "DefaultConsumer" });
}

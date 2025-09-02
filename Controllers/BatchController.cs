using Microsoft.AspNetCore.Mvc;
using S_B_MediaApi.Models;

namespace S_B_MediaApi.Controllers;

[ApiController]
public class BatchController : ControllerBase
{
    [HttpPost("contracts/batchRequest")]
    public IActionResult ContractsBatch([FromBody] IEnumerable<ContractDetail> contracts)
    {
        return StatusCode(201);
    }

    [HttpPost("consumers/batchRequest")]
    public IActionResult ConsumersBatch([FromBody] IEnumerable<ConsumerDetail> consumers)
    {
        return StatusCode(201);
    }

    [HttpGet("batchRequest/{id:int}")]
    public ActionResult<object> GetBatchStatus(int id) => Ok(new { id, status = "done" });
}
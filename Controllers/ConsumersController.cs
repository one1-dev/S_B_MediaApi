using Microsoft.AspNetCore.Mvc;
using S_B_MediaApi.Models;
using S_B_MediaApi.Data;
using System.ComponentModel.DataAnnotations;

namespace S_B_MediaApi.Controllers;

[ApiController]
[Route("consumers")]
public class ConsumersController : ControllerBase
{
    [HttpGet]
    public ActionResult<ConsumerCollection> GetConsumers([FromQuery] long? minContractId, [FromQuery] long? maxContractId,
        [FromQuery] long? minId, [FromQuery] long? maxId, [FromQuery] string? name, [FromQuery] int? status,
        [FromQuery(Name = "delete")] int? deleteMark)
    {
        var list = DemoData.Consumers.ToList();
        if (list.Count == 0) return NoContent();
        return Ok(new ConsumerCollection(list));
    }

    [HttpGet("{Cid:long},{id:long}")]
    public ActionResult<Consumer> GetConsumer(long Cid, long id)
    {
        var c = DemoData.Consumers.FirstOrDefault(x => x.ContractId == Cid && x.Id == id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpDelete("{Cid:long},{id:long}")]
    public IActionResult DeleteConsumer(long Cid, long id)
    {
        var removed = DemoData.Consumers.RemoveAll(x => x.ContractId == Cid && x.Id == id);
        return removed == 0 ? NotFound() : Ok();
    }

    [HttpGet("{Cid:long},{id:long}/detail")]
    public ActionResult<ConsumerDetail> GetConsumerDetail(long Cid, long id)
    {
        var c = DemoData.ConsumerDetails.FirstOrDefault(x => x.Consumer.ContractId == Cid && x.Consumer.Id == id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPut("{Cid:long},{id:long}/detail")]
    public ActionResult<ConsumerDetail> UpdateConsumerDetail(long Cid, long id, [FromBody, Required] ConsumerDetail body)
    {
        var idx = DemoData.ConsumerDetails.FindIndex(x => x.Consumer.ContractId == Cid && x.Consumer.Id == id);
        if (idx < 0) return NotFound();
        var updated = body with { Consumer = body.Consumer with { ContractId = Cid, Id = id } };
        DemoData.ConsumerDetails[idx] = updated;
        return Ok(updated);
    }

    [HttpGet("{Cid:long},{id:long}/sales")]
    public ActionResult<SalesTransactions> GetConsumerSales(long Cid, long id, [FromQuery] DateTime? minDate, [FromQuery] DateTime? maxDate)
    {
        var list = DemoData.Sales.Where(s => s.ContractId == Cid && s.ConsumerId == id).ToList();
        return Ok(new SalesTransactions(list));
    }

    [HttpPost("{Cid:long},{id:long}/sales")]
    public ActionResult<SalesTransactions> CreateConsumerSales(long Cid, long id, [FromBody, Required] SalesTransactions body)
    {
        var added = body.Items.Select(s => s with { ContractId = Cid, ConsumerId = id }).ToList();
        DemoData.Sales.AddRange(added);
        return Created($"/consumers/{Cid},{id}/sales", new SalesTransactions(added));
    }

    [HttpPut("sales")]
    public ActionResult<SalesTransactions> UpdateConsumerSales([FromBody, Required] SalesTransactions body)
    {
        return Ok(body);
    }

    [HttpGet("{Cid:long},{id:long}/parktrans")]
    public ActionResult<ParkTransactions> GetConsumerParkTrans(long Cid, long id, [FromQuery] DateTime? minDate, [FromQuery] DateTime? maxDate)
    {
        var list = DemoData.Park.Where(p => p.ContractId == Cid && p.ConsumerId == id).ToList();
        return Ok(new ParkTransactions(list));
    }
}
using Microsoft.AspNetCore.Mvc;
using S_B_MediaApi.Models;
using S_B_MediaApi.Data;

namespace S_B_MediaApi.Controllers;

[ApiController]
[Route("invoices")]
public class InvoicesController : ControllerBase
{
    [HttpGet]
    public ActionResult<InvoiceCollection> GetInvoices([FromQuery] long? minId, [FromQuery] long? maxId, [FromQuery] DateTime? minDate, [FromQuery] DateTime? maxDate)
    {
        var list = DemoData.Invoices.ToList();
        if (list.Count == 0) return NoContent();
        return Ok(new InvoiceCollection(list));
    }

    [HttpGet("{id:long}")]
    public ActionResult<Invoice> GetInvoice(long id)
    {
        var inv = DemoData.Invoices.FirstOrDefault(i => i.Id == id);
        return inv is null ? NotFound() : Ok(inv);
    }

    [HttpPost]
    public IActionResult CreateInvoice([FromBody] Invoice? body)
    {
        return StatusCode(201);
    }
}
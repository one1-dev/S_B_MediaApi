using Microsoft.AspNetCore.Mvc;
using S_B_MediaApi.Models;
using S_B_MediaApi.Data;
using System.ComponentModel.DataAnnotations;

namespace S_B_MediaApi.Controllers;

[ApiController]
[Route("contracts")]
public class ContractsController : ControllerBase
{
    private static readonly List<ContractDetail> _contracts = new()
    {
        new ContractDetail
        {
            Contract = new Contract { Id = 1, Name = "Acme Holdings", XValidFrom = "2025-01-01" },
            ContractNo = "C-0001",
            Person = new Person { FirstName = "Ada", Surname = "Lovelace" },
            StdAddr = new Address{ Street = "1 Main", Town = "Austin", Country = "US" },
            Status = 0
        }
    };

    [HttpGet]
    public ActionResult GetContracts([FromQuery] long? minId, [FromQuery] long? maxId,
        [FromQuery] string? validFrom, [FromQuery] string? validUntil, [FromQuery] string? name,
        [FromQuery] string? contractNo, [FromQuery] int? status, [FromQuery(Name = "delete")] int? deleteMark)
    {
        var items = _contracts.Select(c => c.Contract).AsEnumerable();
        if (minId is not null) items = items.Where(c => c.Id >= minId);
        if (maxId is not null) items = items.Where(c => c.Id <= maxId);
        if (!string.IsNullOrWhiteSpace(name)) items = items.Where(c => c.Name.Contains(name!, StringComparison.OrdinalIgnoreCase));
        var list = items.ToList();
        if (list.Count == 0) return NoContent();
        return Ok(new ContractCollection(list));
    }

    [HttpPost]
    public ActionResult<ContractDetail> CreateContract([FromBody, Required] ContractDetail body, [FromQuery] int? templateId)
    {
        var nextId = (_contracts.Max(c => (long?)c.Contract.Id) ?? 0) + 1;
        var created = body with { Contract = body.Contract with { Id = nextId } };
        _contracts.Add(created);
        return Created($"/contracts/{nextId}/detail", created);
    }

    [HttpGet("{id:long}")]
    public ActionResult GetContract(long id)
    {
        var found = _contracts.FirstOrDefault(c => c.Contract.Id == id);
        return found is null ? NotFound() : Ok(new ContractCollection(new[] { found.Contract }));
    }

    [HttpDelete("{id:long}")]
    public IActionResult DeleteContract(long id)
    {
        var removed = _contracts.RemoveAll(c => c.Contract.Id == id);
        return removed == 0 ? NotFound() : Ok();
    }

    [HttpGet("{id:long}/detail")]
    public ActionResult<ContractDetail> GetContractDetail(long id)
    {
        var found = _contracts.FirstOrDefault(c => c.Contract.Id == id);
        return found is null ? NotFound() : Ok(found);
    }

    [HttpPut("{id:long}/detail")]
    public ActionResult<ContractDetail> UpdateContractDetail(long id, [FromBody, Required] ContractDetail body)
    {
        var idx = _contracts.FindIndex(c => c.Contract.Id == id);
        if (idx < 0) return NotFound();
        var updated = body with { Contract = body.Contract with { Id = id } };
        _contracts[idx] = updated;
        return Ok(updated);
    }

    [HttpGet("{id:long}/consumers")]
    public ActionResult<ConsumerCollection> GetContractConsumers(long id)
    {
        var list = DemoData.Consumers.Where(c => c.ContractId == id).ToList();
        if (list.Count == 0) return NoContent();
        return Ok(new ConsumerCollection(list));
    }

    [HttpGet("{id:long}/consumersdetail")]
    public ActionResult<ConsumerDetails> GetContractConsumersDetail(long id)
    {
        var details = DemoData.ConsumerDetails.Where(cd => cd.Consumer.ContractId == id).ToList();
        return Ok(new ConsumerDetails(details));
    }

    [HttpPost("{id:long}/consumers")]
    public ActionResult<ConsumerDetail> CreateContractConsumer(long id, [FromBody, Required] ConsumerDetail body, [FromQuery] int? templateId)
    {
        var nextId = (DemoData.Consumers.Where(c => c.ContractId == id).Max(c => (long?)c.Id) ?? 0) + 1;
        var created = body with { Consumer = body.Consumer with { Id = nextId, ContractId = id } };
        DemoData.Consumers.Add(new Consumer { Id = created.Consumer.Id, ContractId = id, Name = created.Consumer.Name, FilialId = created.Consumer.FilialId });
        DemoData.ConsumerDetails.Add(created);
        return Created($"/consumers/{id},{nextId}/detail", created);
    }

    [HttpGet("{id:long}/invoices")]
    public ActionResult<InvoiceCollection> GetContractInvoices(long id, [FromQuery] long? minId, [FromQuery] long? maxId, [FromQuery] DateTime? minDate, [FromQuery] DateTime? maxDate)
    {
        var list = DemoData.Invoices.Where(i => i.CompanyRef == id);
        if (minId is not null) list = list.Where(i => i.Id >= minId);
        if (maxId is not null) list = list.Where(i => i.Id <= maxId);
        if (minDate is not null) list = list.Where(i => i.InvcDate >= minDate);
        if (maxDate is not null) list = list.Where(i => i.InvcDate <= maxDate);
        var result = list.ToList();
        if (result.Count == 0) return NoContent();
        return Ok(new InvoiceCollection(result));
    }

    [HttpPost("{id:long}/invoices")]
    public ActionResult<InvoiceJob> CreateContractInvoiceJob(long id, [FromQuery] DateTime? startDateTime, [FromQuery] DateTime? refDate, [FromQuery] int? flatRate, [FromQuery] int? bestPrice)
    {
        var job = new InvoiceJob(12345, $"/jobs/12345");
        return Created(job.Href, job);
    }

    [HttpGet("{id:long}/account")]
    public ActionResult<Account> GetContractAccount(long id, [FromQuery] DateTime? minDate, [FromQuery] DateTime? maxDate)
    {
        var tx = new List<AccountTransaction>
        {
            new() { Id = 1, TransactType = 0, CreateTime = DateTime.UtcNow.AddDays(-2), BookingTime = DateTime.UtcNow.AddDays(-1), Amount = 0 }
        };
        return Ok(new Account(tx));
    }

    [HttpGet("{id:long}/sales")]
    public ActionResult<SalesTransactions> GetContractSales(long id, [FromQuery] DateTime? minDate, [FromQuery] DateTime? maxDate, [FromQuery] int exported = -1)
    {
        var list = DemoData.Sales.Where(s => s.ContractId == id).ToList();
        return list.Count == 0 ? NoContent() : Ok(new SalesTransactions(list));
    }

    [HttpPost("{id:long}/sales")]
    public ActionResult<SalesTransactions> CreateContractSales(long id, [FromBody, Required] SalesTransactions body)
    {
        var added = body.Items.Select(s => s with { ContractId = id }).ToList();
        DemoData.Sales.AddRange(added);
        return Created($"/contracts/{id}/sales", new SalesTransactions(added));
    }

    [HttpPut("sales")]
    public ActionResult<SalesTransactions> UpdateContractSales([FromBody, Required] SalesTransactions body)
    {
        return Ok(body);
    }

    [HttpGet("{id:long}/parktrans")]
    public ActionResult<ParkTransactions> GetContractParkTrans(long id, [FromQuery] DateTime? minDate, [FromQuery] DateTime? maxDate)
    {
        var list = DemoData.Park.Where(p => p.ContractId == id).ToList();
        return list.Count == 0 ? NoContent() : Ok(new ParkTransactions(list));
    }

    [HttpGet("{id:long}/freefacilities")]
    public ActionResult<ContractFreeFacilities> GetFreeFacilities(long id)
    {
        return Ok(new ContractFreeFacilities(new[]
        {
            new ContractFreeFacility{ XValidFrom = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"), XValidUntil = DateTime.UtcNow.ToString("yyyy-MM-dd"), FacilityId = 2011 }
        }));
    }

    [HttpPost("{id:long}/freefacilities")]
    public IActionResult CreateFreeFacilities(long id, [FromBody, Required] ContractFreeFacilities body)
    {
        return StatusCode(201);
    }

    [HttpDelete("{id:long}/freefacilities")]
    public IActionResult DeleteFreeFacilities(long id)
    {
        return Ok();
    }
}
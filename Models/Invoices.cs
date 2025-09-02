namespace S_B_MediaApi.Models;


public record Invoice
{
public long Id { get; init; }
public DateTime? InvcDate { get; init; }
public decimal? InvcAmount { get; init; }
public long? CompanyRef { get; init; }
public string? CompanyUrl { get; init; }
}


public record InvoiceCollection(IEnumerable<Invoice> Invoices);


public record InvoiceJob(int JobId, string Href);
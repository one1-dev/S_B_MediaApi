namespace S_B_MediaApi.Models;


public record AccountTransaction
{
public long Id { get; init; }
public int? TransactType { get; init; }
public DateTime? CreateTime { get; init; }
public DateTime? BookingTime { get; init; }
public decimal? Amount { get; init; }
}


public record Account(IEnumerable<AccountTransaction> AccountItems);
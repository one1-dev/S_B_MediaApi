namespace S_B_MediaApi.Models;


public record ParkTransaction
{
public DateTime? TransactionTime { get; init; }
public int? TransactionType { get; init; }
public int? FacilityIn { get; init; }
public int? FacilityOut { get; init; }
public int? Computer { get; init; }
public int? Device { get; init; }
public decimal? Amount { get; init; }
public long? ContractId { get; init; }
public long? ConsumerId { get; init; }
}


public record ParkTransactions(IEnumerable<ParkTransaction> Items);
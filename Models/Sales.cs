namespace S_B_MediaApi.Models;


public record SalesTransaction
{
public long? ContractId { get; init; }
public long? ConsumerId { get; init; }
public int? ArticleId { get; init; }
public int? ArticleClass { get; init; }
public string? ArticleName { get; init; }
public decimal? Quantity { get; init; }
public decimal? SinglePrice { get; init; }
public decimal? TotalAmount { get; init; }
public DateTime? ServiceTime { get; init; }
public DateTime? SellTime { get; init; }
}


public record SalesTransactions(IEnumerable<SalesTransaction> Items);
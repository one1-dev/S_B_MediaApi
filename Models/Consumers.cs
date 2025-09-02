namespace S_B_MediaApi.Models;


public record Consumer
{
public long Id { get; init; }
public long ContractId { get; init; }
public string? Name { get; init; }
public long? FilialId { get; init; }
}


public record ConsumerCollection(IEnumerable<Consumer> Consumers);


public record Identification
{
public int? PtcptType { get; init; }
public string? CardNo { get; init; }
}


public record ConsumerDetail
{
public Consumer Consumer { get; init; } = new();
public Person? Person { get; init; }
public Identification? Identification { get; init; }
public int? Status { get; init; }
public int? Delete { get; init; }
}


public record ConsumerDetails(IEnumerable<ConsumerDetail> Items);
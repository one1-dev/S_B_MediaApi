namespace S_B_MediaApi.Models;


public record Contract
{
public long Id { get; init; }
public string Name { get; init; } = string.Empty;
public string? XValidFrom { get; init; }
public string? XValidUntil { get; init; }
public long? FilialId { get; init; }
}


public record ContractCollection(IEnumerable<Contract> Contracts);


public record PaymentInfo
{
public int? PaymentType { get; init; }
}


public record ContractDetail
{
public Contract Contract { get; init; } = new();
public string? ContractNo { get; init; }
public Person? Person { get; init; }
public Address? StdAddr { get; init; }
public Address? DeliveryAddr { get; init; }
public PaymentInfo? PaymentInfo { get; init; }
public int? Status { get; init; }
public int? Delete { get; init; }
}


public record ContractFreeFacility
{
public string? XValidFrom { get; init; }
public string? XValidUntil { get; init; }
public int? FacilityId { get; init; }
}


public record ContractFreeFacilities(IEnumerable<ContractFreeFacility> Items);
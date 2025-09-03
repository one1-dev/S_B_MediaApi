using System.Text.Json.Serialization;

namespace S_B_MicroService.Integrations.Sb.Models;

// Payload we send to S&B to update a contract
public sealed record ContractDetailUpdate(
    [property: JsonPropertyName("licensePlate")] string LicensePlate,
    [property: JsonPropertyName("validFrom")] DateTime? ValidFrom,
    [property: JsonPropertyName("name")] string? Name
);

// Sample subset of fields we read from S&B
public sealed record ContractDetailResponse(
    [property: JsonPropertyName("contract")] ContractData Contract
);

public sealed record ContractData(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("licensePlate")] string? LicensePlate,
    [property: JsonPropertyName("validFrom")] DateTime? ValidFrom,
    [property: JsonPropertyName("name")] string? Name
);

public sealed record ConsumersDetailResponse(
    [property: JsonPropertyName("present")] bool? Present,
    [property: JsonPropertyName("lpn")] string? LicensePlate
);

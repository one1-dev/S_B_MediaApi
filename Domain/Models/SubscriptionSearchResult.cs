namespace S_B_MicroService.Domain.Models;

public sealed record SubscriptionSearchResult(
    string SubscriptionId,
    string EmployeeName,
    string CurrentLicense,
    string SubscriptionType,
    string SbContractId
);

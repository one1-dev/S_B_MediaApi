namespace S_B_MicroService.Domain.Models;

public sealed record BulkUploadResult(
    string OfficeId,
    int SuccessCount,
    int FailureCount,
    IReadOnlyList<BulkRowResult> Rows
);

public sealed record BulkRowResult(
    int Line,
    string EmployeeName,
    string LicensePlate,
    int? ContractId,
    bool Success,
    string? Error
);

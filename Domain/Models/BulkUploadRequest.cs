using Microsoft.AspNetCore.Http;

namespace S_B_MicroService.Domain.Models;

public sealed class BulkUploadRequest
{
    public IFormFile File { get; set; } = default!;
    public string OfficeId { get; set; } = string.Empty;
}

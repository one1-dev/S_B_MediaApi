using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S_B_MicroService.Domain.Models;
using S_B_MicroService.Domain.Services;

namespace S_B_MicroService.Controllers;

[ApiController]
[Route("api/parking/subscriptions")]
public sealed class BulkUploadController(ISubscriptionsService svc, ILogger<BulkUploadController> logger) : ControllerBase
{
    [HttpPost("bulk-upload")]
    [Authorize(Policy = "Contracts.Write")]
    [RequestSizeLimit(10_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BulkUploadResult>> BulkUpload([FromForm] BulkUploadRequest request, CancellationToken ct)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest(new { error = "empty_file" });

        var results = new List<BulkRowResult>();

        using var reader = new StreamReader(request.File.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

        // optional header
        var _ = await reader.ReadLineAsync();
        var lineNo = 1;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            lineNo++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cols = line.Split(',', StringSplitOptions.TrimEntries);
            var employee = Get(cols, 0);   // employee_name
            var license = Get(cols, 2);   // license_plate (per your CSV example)
            var contractStr = Get(cols, 4);   // contract_id if present (else leave empty)

            if (string.IsNullOrWhiteSpace(license))
            {
                results.Add(new BulkRowResult(lineNo, employee, license, null, false, "missing_license_plate"));
                continue;
            }

            int? contractId = TryParseContractId(contractStr, out var id) ? id : null;

            try
            {
                if (contractId is null)
                {
                    // In your real flow you can resolve by office/employee mapping
                    results.Add(new BulkRowResult(lineNo, employee, license, null, false, "missing_contract_id"));
                    continue;
                }

                await svc.UpdateLicensePlateAsync(contractId.Value, license, ct);
                results.Add(new BulkRowResult(lineNo, employee, license, contractId, true, null));

                // throttle to avoid vendor rate limiting
                await Task.Delay(1000, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Bulk upload failed at line {Line}", lineNo);
                results.Add(new BulkRowResult(lineNo, employee, license, contractId, false, ex.Message));
            }
        }

        var ok = results.Count(r => r.Success);
        var fail = results.Count - ok;

        return Ok(new BulkUploadResult(request.OfficeId, ok, fail, results));
    }

    private static string Get(string[] arr, int idx) => idx < arr.Length ? arr[idx].Trim() : string.Empty;

    private static bool TryParseContractId(string s, out int id)
    {
        id = 0;
        if (string.IsNullOrWhiteSpace(s)) return false;
        var digits = new string(s.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out id);
    }
}

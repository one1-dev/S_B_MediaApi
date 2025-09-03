using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S_B_MicroService.Domain.Models;
using S_B_MicroService.Domain.Services;
using S_B_MicroService.Integrations.Sb;

namespace S_B_MicroService.Controllers;

[ApiController]
[Route("api/parking")]
public sealed class ParkingSubscriptionsController(ISubscriptionsService svc, ISbClient sb) : ControllerBase
{
    // Scenario 1.1 step 2 – search subscription (stub; replace with DB/OpenSearch)
    [HttpGet("subscriptions/search")]
    [Authorize]
    public ActionResult<SubscriptionSearchResult> Search([FromQuery] string office_id, [FromQuery] string query)
    {
        // TODO: query your DB; stubbed example:
        return Ok(new SubscriptionSearchResult(
            "12345", "דוד לוי", "123-45-678", "שמור", "C001234"
        ));
    }

    // Scenario 1.1 step 3 – vehicle present / last-seen (S&B or cache)
    [HttpGet("vehicle-status")]
    [Authorize]
    public async Task<ActionResult<VehicleStatusResponse>> VehicleStatus(
        [FromQuery(Name = "license_plate")] string licensePlate,
        [FromQuery(Name = "contract_id")] int contractId,
        CancellationToken ct)
    {
        // Reads consumer detail from S&B (or adapt to your cache)
        var detail = await sb.GetConsumersDetailAsync(contractId, licensePlate, ct);
        var present = detail?.Present ?? false;

        return Ok(new VehicleStatusResponse(
            Present: present,
            LicensePlate: licensePlate,
            ContractId: contractId,
            Message: present
                ? "הרכב נמצא בחניון כרגע"
                : "הרכב אינו נמצא בחניון"
        ));
    }

    // Scenario 1.1 step 4 – update license plate in S&B (JSON)
    [HttpPut("subscriptions/{contractId:int}/license-plate")]
    [Authorize(Policy = "Contracts.Write")]
    public async Task<IActionResult> UpdatePlate(
        int contractId,
        [FromBody] UpdatePlateDto body,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.NewLicense))
            return BadRequest(new { error = "missing_new_license" });

        await svc.UpdateLicensePlateAsync(contractId, body.NewLicense, ct);

        return Ok(new
        {
            message = "מספר הרישוי עודכן בהצלחה",
            newLicense = body.NewLicense
        });
    }

    // Scenario 1.2 – bulk CSV upload (rate-limited updates to S&B)
    // Expecting CSV columns (recommended): employee_name,license_plate,contract_id
    // If contract_id is missing -> row fails (you can enrich from DB if you have mapping)
    [HttpPost("subscriptions/bulk-upload")]
    [Authorize(Policy = "Contracts.Write")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<BulkUploadResult>> BulkUpload(
        [FromForm] IFormFile file,
        [FromForm] string office_id,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "empty_file" });

        var results = new List<BulkRowResult>();

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

        // header (optional)
        var header = await reader.ReadLineAsync();
        var lineNo = 1;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            lineNo++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cols = SplitCsv(line);
            var employee = Get(cols, 0);
            var license = Get(cols, 1);
            var contractStr = Get(cols, 2);

            if (string.IsNullOrWhiteSpace(employee) || string.IsNullOrWhiteSpace(license))
            {
                results.Add(new BulkRowResult(lineNo, employee, license, null, false, "missing_required_fields"));
                continue;
            }

            if (!TryParseContractId(contractStr, out var contractId))
            {
                results.Add(new BulkRowResult(lineNo, employee, license, null, false, "missing_contract_id"));
                continue;
            }

            try
            {
                await svc.UpdateLicensePlateAsync(contractId, license, ct);
                results.Add(new BulkRowResult(lineNo, employee, license, contractId, true, null));

                // Throttle: 1 req/sec to avoid vendor rate limits
                await Task.Delay(1000, ct);
            }
            catch (Exception ex)
            {
                results.Add(new BulkRowResult(lineNo, employee, license, contractId, false, ex.Message));
            }
        }

        var ok = results.Count(r => r.Success);
        var fail = results.Count - ok;

        return Ok(new BulkUploadResult(office_id, ok, fail, results));
    }

    // --- helpers & local DTOs ---

    private static string Get(string[] arr, int idx) =>
        idx < arr.Length ? arr[idx].Trim() : string.Empty;

    private static bool TryParseContractId(string s, out int id) =>
        int.TryParse(new string((s ?? "").Where(char.IsDigit).ToArray()), out id);

    private static string[] SplitCsv(string line)
    {
        // Minimal CSV splitter (no quoted-commas handling).
        // Replace with a CSV library if you need robust parsing.
        return line.Split(',', StringSplitOptions.TrimEntries);
    }
}

// Request/Response DTOs local to this controller
public sealed record UpdatePlateDto(string NewLicense);

public sealed record VehicleStatusResponse(
    bool Present,
    string LicensePlate,
    int ContractId,
    string Message
);

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

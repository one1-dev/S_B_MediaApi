using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S_B_MicroService.Integrations.Sb;

namespace S_B_MicroService.Controllers;

[ApiController]
[Route("api/parking")]
public sealed class VehicleController(ISbClient sb, ILogger<VehicleController> logger) : ControllerBase
{
    /// <summary>
    /// Returns whether the vehicle is currently present in the parking lot
    /// (based on S&B consumer detail) and echoes the identifiers.
    /// </summary>
    /// <param name="licensePlate">License plate (e.g., 123-45-678)</param>
    /// <param name="contractId">S&B contract id (e.g., C001234 → 1234)</param>
    [HttpGet("vehicle-status")]
    [Authorize]
    public async Task<ActionResult<VehicleStatusDto>> VehicleStatus(
        [FromQuery(Name = "license_plate")] string licensePlate,
        [FromQuery(Name = "contract_id")] int contractId,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            return BadRequest(new { error = "missing_license_plate" });

        // Query downstream S&B (JSON client)
        var detail = await sb.GetConsumersDetailAsync(contractId, licensePlate, ct);
        var present = detail?.Present ?? false;

        var result = new VehicleStatusDto(
            Present: present,
            LicensePlate: licensePlate,
            ContractId: contractId,
            Message: present ? "הרכב נמצא בחניון כרגע" : "הרכב אינו נמצא בחניון"
        );

        logger.LogInformation("VehicleStatus {@Result}", result);
        return Ok(result);
    }
}

/* ====== DTOs (top-level to avoid Swagger schema collisions) ====== */

public sealed record VehicleStatusDto(
    bool Present,
    string LicensePlate,
    int ContractId,
    string Message
);

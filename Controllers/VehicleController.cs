using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S_B_MicroService.Integrations.Sb;

namespace S_B_MicroService.Controllers;

[ApiController]
[Route("api/parking")]
public sealed class VehicleController(ISbClient sb, ILogger<VehicleController> logger) : ControllerBase
{
    [HttpGet("vehicle-status")]
    [Authorize]
    public async Task<ActionResult<VehicleStatusDto>> VehicleStatus(
        [FromQuery(Name = "license_plate")] string licensePlate,
        [FromQuery(Name = "contract_id")] int contractId,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            return BadRequest(new { error = "missing_license_plate" });

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

public sealed record VehicleStatusDto(
    bool Present,
    string LicensePlate,
    int ContractId,
    string Message
);

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace S_B_MicroService.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly ILogger<ReportsController> _log;
    public ReportsController(ILogger<ReportsController> log) => _log = log;

    [HttpGet("subscriptions-by-office")]
    [Authorize]
    public ActionResult<SubscriptionsByOfficeResponse> SubscriptionsByOffice(
        [FromQuery(Name = "property_id")] string propertyId,
        [FromQuery] string? date // YYYY-MM-DD (optional)
    )
    {
        // TODO: replace with real data aggregation
        var offices = new List<OfficeSubscriptions>
        {
            new(
                OfficeName: "עו\"ד כהן ושות'",
                Subscriptions: new OfficeSubscriptionCounts(
                    PrivateZ2a: 12, Floating: 5, Reserved: 8, Total: 25, ChargingStations: 3
                )
            ),
            new(
                OfficeName: "חברת הייטק אלפא",
                Subscriptions: new OfficeSubscriptionCounts(
                    PrivateZ2a: 45, Floating: 20, Reserved: 35, Total: 100, ChargingStations: 15
                )
            )
        };

        return Ok(new SubscriptionsByOfficeResponse(offices));
    }

    [HttpGet("realtime/occupancy")]
    [Authorize]
    public ActionResult<OccupancyResponse> RealtimeOccupancy(
        [FromQuery] string location,
        [FromQuery] string level = "office"
    )
    {
        // TODO: replace with live feed
        var offices = new List<OfficeOccupancy>
        {
            new("עו\"ד כהן ושות'", InLot: 18, Capacity: 25),
            new("חברת הייטק אלפא", InLot: 87, Capacity: 100)
        };

        var totalIn = offices.Sum(o => o.InLot);
        var totalCap = offices.Sum(o => o.Capacity);
        var util = totalCap == 0 ? 0 : Math.Round(100.0 * totalIn / totalCap, 1);

        return Ok(new OccupancyResponse(
            Location: location,
            Level: level,
            Offices: offices.Select(o => o with { Utilization = Percent(o.InLot, o.Capacity) }).ToList(),
            TotalIn: totalIn,
            TotalCapacity: totalCap,
            Utilization: util
        ));

        static double Percent(int num, int den) => den == 0 ? 0 : Math.Round(100.0 * num / den, 1);
    }
}

/* ============================
 * Response DTOs (top-level)
 * ============================ */

public sealed record SubscriptionsByOfficeResponse(
    [property: JsonPropertyName("offices")] IReadOnlyList<OfficeSubscriptions> Offices
);

public sealed record OfficeSubscriptions(
    [property: JsonPropertyName("office_name")] string OfficeName,
    [property: JsonPropertyName("subscriptions")] OfficeSubscriptionCounts Subscriptions
);

public sealed record OfficeSubscriptionCounts(
    [property: JsonPropertyName("private_z2a")] int PrivateZ2a,
    [property: JsonPropertyName("floating")] int Floating,
    [property: JsonPropertyName("reserved")] int Reserved,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("charging_stations")] int ChargingStations
);

public sealed record OccupancyResponse(
    [property: JsonPropertyName("location")] string Location,
    [property: JsonPropertyName("level")] string Level,
    [property: JsonPropertyName("offices")] IReadOnlyList<OfficeOccupancy> Offices,
    [property: JsonPropertyName("total_in")] int TotalIn,
    [property: JsonPropertyName("total_capacity")] int TotalCapacity,
    [property: JsonPropertyName("utilization")] double Utilization
);

public sealed record OfficeOccupancy(
    [property: JsonPropertyName("office_name")] string OfficeName,
    [property: JsonPropertyName("in_lot")] int InLot,
    [property: JsonPropertyName("capacity")] int Capacity,
    [property: JsonPropertyName("utilization")] double Utilization = 0
);

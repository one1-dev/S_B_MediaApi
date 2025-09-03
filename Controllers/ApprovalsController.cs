using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace S_B_MicroService.Controllers;

[ApiController]
[Route("api/approvals")]
public sealed class ApprovalsController : ControllerBase
{
    private static readonly ConcurrentDictionary<Guid, ApprovalItem> _store = new();
    private readonly ILogger<ApprovalsController> _log;

    public ApprovalsController(ILogger<ApprovalsController> log) => _log = log;

    /// <summary>
    /// Submit a parking-change request (office not authorized for direct update).
    /// </summary>
    [HttpPost("parking-change")]
    [Authorize] // adjust to your policy if needed
    public ActionResult<ApprovalItem> Submit([FromBody] ParkingChangeRequestDto req)
    {
        if (string.IsNullOrWhiteSpace(req.OfficeName) ||
            string.IsNullOrWhiteSpace(req.Employee) ||
            string.IsNullOrWhiteSpace(req.CurrentLicense) ||
            string.IsNullOrWhiteSpace(req.RequestedLicense))
        {
            return BadRequest(new { error = "missing_required_fields" });
        }

        var item = new ApprovalItem
        {
            Id = Guid.NewGuid(),
            OfficeName = req.OfficeName,
            Employee = req.Employee,
            CurrentLicense = req.CurrentLicense,
            RequestedLicense = req.RequestedLicense,
            Reason = req.Reason,
            Status = ApprovalStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };

        _store[item.Id] = item;
        _log.LogInformation("Approval submitted {@Approval}", item);

        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    /// <summary>
    /// Get approval by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    public ActionResult<ApprovalItem> Get(Guid id)
    {
        return _store.TryGetValue(id, out var item) ? Ok(item) : NotFound();
    }

    /// <summary>
    /// Approve request.
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    [Authorize(/*Policy = "Approvals.Manage"*/)]
    public ActionResult<ApprovalItem> Approve(Guid id, [FromBody] DecisionDto? body)
    {
        if (!_store.TryGetValue(id, out var item)) return NotFound();

        if (item.Status != ApprovalStatus.Pending)
            return Conflict(new { error = "already_decided", status = item.Status.ToString() });

        item.Status = ApprovalStatus.Approved;
        item.DecidedAtUtc = DateTime.UtcNow;
        item.DecidedBy = User.Identity?.Name ?? "system";
        item.DecisionNote = body?.Note;

        _log.LogInformation("Approval APPROVED {@Approval}", item);
        return Ok(item);
    }

    /// <summary>
    /// Reject request.
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [Authorize(/*Policy = "Approvals.Manage"*/)]
    public ActionResult<ApprovalItem> Reject(Guid id, [FromBody] DecisionDto? body)
    {
        if (!_store.TryGetValue(id, out var item)) return NotFound();

        if (item.Status != ApprovalStatus.Pending)
            return Conflict(new { error = "already_decided", status = item.Status.ToString() });

        item.Status = ApprovalStatus.Rejected;
        item.DecidedAtUtc = DateTime.UtcNow;
        item.DecidedBy = User.Identity?.Name ?? "system";
        item.DecisionNote = body?.Note ?? "rejected";

        _log.LogInformation("Approval REJECTED {@Approval}", item);
        return Ok(item);
    }
}

// ===== DTOs / models (you can move these to Domain/Models later) =====

public sealed record ParkingChangeRequestDto(
    string OfficeName,
    string Employee,
    string CurrentLicense,
    string RequestedLicense,
    string? Reason
);

public sealed record DecisionDto(string? Note);

public enum ApprovalStatus { Pending = 0, Approved = 1, Rejected = 2 }

public sealed class ApprovalItem
{
    public Guid Id { get; set; }
    public string OfficeName { get; set; } = "";
    public string Employee { get; set; } = "";
    public string CurrentLicense { get; set; } = "";
    public string RequestedLicense { get; set; } = "";
    public string? Reason { get; set; }

    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
    public string? DecidedBy { get; set; }
    public string? DecisionNote { get; set; }
}

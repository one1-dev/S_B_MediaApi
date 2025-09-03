namespace S_B_MicroService.Models;


public record UsageProfile
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Href { get; init; }
}
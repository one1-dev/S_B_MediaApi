public record OsHitDto(
    double? Score,
    string? Name,
    IReadOnlyDictionary<string, IReadOnlyCollection<string>>? Highlights
);
namespace S_B_MediaApi.Models;


public record VersionEnvelope(string Version);
public record ResourceList(IEnumerable<string> Resources);
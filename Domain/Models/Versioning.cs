namespace S_B_MicroService.Models;


public record VersionEnvelope(string Version);
public record ResourceList(IEnumerable<string> Resources);
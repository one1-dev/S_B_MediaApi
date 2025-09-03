namespace S_B_MicroService.Domain.Models;

public class ContractEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}

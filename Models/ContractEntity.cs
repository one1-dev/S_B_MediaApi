namespace S_B_MediaApi.Models;

public class ContractEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}

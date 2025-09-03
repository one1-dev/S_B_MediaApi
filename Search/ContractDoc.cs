namespace S_B_MicroService.Search;

public class ContractDoc
{
    public long ContractId { get; set; }
    public string Name { get; set; } = default!;
    public string? Memo { get; set; }
    public DateTime IndexedAtUtc { get; set; }
}

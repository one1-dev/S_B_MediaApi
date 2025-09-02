using Microsoft.EntityFrameworkCore;
using OpenSearch.Client;
using S_B_MediaApi.Data;
using S_B_MediaApi.Models;
using S_B_MediaApi.Search;

namespace S_B_MediaApi.Services;

public class ContractSearchService
{
    private readonly IOpenSearchClient _os;
    private readonly AppDbContext _db;

    public ContractSearchService(IOpenSearchClient os, AppDbContext db)
    {
        _os = os;
        _db = db;
    }

    public async Task<object?> GetContractWithHighlightsAsync(long id, CancellationToken ct = default)
    {
        var contract = await _db.Contracts.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (contract is null) return null;

        var osResp = await _os.SearchAsync<ContractDoc>(s => s
    .Index("contracts")
    .Size(5)
    .Query(q => q.Term(t => t.Field(f => f.ContractId).Value(id)))
    .Highlight(h => h.Fields(hf => hf.Field(f => f.Name))),
    ct);

        List<OsHitDto> hits = osResp.IsValid
            ? osResp.Hits
                .Select(h => new OsHitDto(
                    h.Score,
                    h.Source?.Name,
                    h.Highlight
                ))
                .ToList()
            : new List<OsHitDto>();

        return new
        {
            contract.Id,
            contract.Name,
            contract.ValidFrom,
            contract.ValidUntil,
            SearchHits = hits
        };
    }

    public Task IndexContractAsync(ContractDoc doc, CancellationToken ct = default) =>
        _os.IndexDocumentAsync(doc, ct);
}
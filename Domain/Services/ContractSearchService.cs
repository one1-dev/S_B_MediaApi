using Microsoft.EntityFrameworkCore;
using OpenSearch.Client;
using S_B_MicroService.Data;
using S_B_MicroService.Models;
using S_B_MicroService.Search;

namespace S_B_MicroService.Domain.Services;

public class ContractSearchService
{
    private readonly IOpenSearchClient _os;
    private readonly AppDbContext _db;

    public ContractSearchService(IOpenSearchClient os, AppDbContext db)
    {
        _os = os;
        _db = db;
    }

    public Task IndexContractAsync(ContractDoc doc, CancellationToken ct = default) =>
        _os.IndexDocumentAsync(doc, ct);
}
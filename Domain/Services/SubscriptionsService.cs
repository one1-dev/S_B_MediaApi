using S_B_MicroService.Integrations.Sb;
using S_B_MicroService.Integrations.Sb.Models;

namespace S_B_MicroService.Domain.Services;

public sealed class SubscriptionsService(ISbClient sb) : ISubscriptionsService
{
    public async Task UpdateLicensePlateAsync(int contractId, string newPlate, CancellationToken ct)
    {
        // TODO: validate permissions, last-change window, etc. (your DB)
        var payload = new ContractDetailUpdate(newPlate, null, null);
        await sb.UpdateContractDetailAsync(contractId, payload, ct);
        // TODO: persist audit to your DB
    }
}

using S_B_MicroService.Integrations.Sb.Models;

namespace S_B_MicroService.Integrations.Sb;

public interface ISbClient
{
    Task<ContractDetailResponse> GetContractDetailAsync(int contractId, CancellationToken ct);
    Task UpdateContractDetailAsync(int contractId, ContractDetailUpdate payload, CancellationToken ct);
    Task<ConsumersDetailResponse> GetConsumersDetailAsync(int contractId, string? lpn, CancellationToken ct);
}

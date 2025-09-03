namespace S_B_MicroService.Domain.Services;

public interface ISubscriptionsService
{
    Task UpdateLicensePlateAsync(int contractId, string newPlate, CancellationToken ct);
}

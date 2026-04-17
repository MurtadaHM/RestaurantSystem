namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface ITeam6OrderSyncService
    {
        Task<int> SyncActiveOrdersAsync(CancellationToken cancellationToken = default);
    }
}
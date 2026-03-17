using RestaurantSystem.Application.DTOs.Tables;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface ITableService
    {
        Task<IEnumerable<TableResponseDto>> GetAllTablesAsync();
        Task<IEnumerable<TableResponseDto>> GetAvailableTablesAsync();
        Task<TableResponseDto> GetTableByIdAsync(Guid id);
        Task<TableResponseDto> CreateTableAsync(CreateTableRequestDto request);
        Task<TableResponseDto> UpdateTableAsync(Guid id, UpdateTableRequestDto request);  // ✅
        Task DeleteTableAsync(Guid id);
    }
}

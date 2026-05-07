using AutoMapper;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.ActivityLogs;
using RestaurantSystem.Application.DTOs.Tables;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Domain.Exceptions;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;
        private readonly IActivityLogService _activityLogService;
        private readonly IMapper _mapper;
        private readonly ILogger<TableService> _logger;

        public TableService(
            ITableRepository tableRepository,
            IActivityLogService activityLogService,
            IMapper mapper,
            ILogger<TableService> logger)
        {
            _tableRepository = tableRepository;
            _activityLogService = activityLogService;
            _mapper = mapper;
            _logger = logger;
        }

        // 1. جلب كافة الطاولات
        public async Task<IEnumerable<TableResponseDto>> GetAllTablesAsync()
        {
            var tables = await _tableRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TableResponseDto>>(tables);
        }

        // 2. جلب الطاولات المتاحة فقط
        public async Task<IEnumerable<TableResponseDto>> GetAvailableTablesAsync()
        {
            var availableTables = await _tableRepository.GetAvailableTablesAsync();
            return _mapper.Map<IEnumerable<TableResponseDto>>(availableTables);
        }

        // 3. جلب طاولة محددة بواسطة الـ ID
        public async Task<TableResponseDto> GetTableByIdAsync(Guid id)
        {
            var table = await _tableRepository.GetByIdWithOrdersAsync(id);

            if (table == null)
                throw new NotFoundException("الطاولة", id);

            return _mapper.Map<TableResponseDto>(table);
        }

        // 4. إنشاء طاولة جديدة
        public async Task<TableResponseDto> CreateTableAsync(CreateTableRequestDto request)
        {
            NormalizeCreateRequest(request);

            var tableNumberExists = await _tableRepository.ExistsByTableNumberAsync(request.TableNumber);
            if (tableNumberExists)
                throw new ConflictException($"رقم الطاولة '{request.TableNumber}' موجود بالفعل.");

            var codeExists = await _tableRepository.ExistsByCodeAsync(request.Code);
            if (codeExists)
                throw new ConflictException($"كود الطاولة '{request.Code}' موجود بالفعل.");

            var table = _mapper.Map<Table>(request);

            table.Status = TableStatus.Available;
            table.CreatedAt = DateTime.UtcNow;

            await _tableRepository.AddAsync(table);

            await SafeLogActivityAsync(new CreateActivityLogDto
            {
                UserId = null,
                UserName = "System",
                UserRole = "System",
                ActionType = ActivityActionType.TableCreated,
                Module = "Tables",
                EntityName = nameof(Table),
                EntityId = table.Id,
                Description = $"Created table '{table.TableNumber}' with code '{table.Code}' in location '{table.Location}'.",
                NewValue = BuildTableValue(table)
            });

            return _mapper.Map<TableResponseDto>(table);
        }

        // 5. تحديث بيانات طاولة موجودة
        public async Task<TableResponseDto> UpdateTableAsync(Guid id, UpdateTableRequestDto request)
        {
            NormalizeUpdateRequest(request);

            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
                throw new NotFoundException("الطاولة", id);

            var oldValue = BuildTableValue(table);

            if (!string.Equals(table.TableNumber, request.TableNumber, StringComparison.OrdinalIgnoreCase))
            {
                var tableNumberExists = await _tableRepository.ExistsByTableNumberAsync(request.TableNumber);
                if (tableNumberExists)
                    throw new ConflictException($"رقم الطاولة '{request.TableNumber}' مستخدم من قبل طاولة أخرى.");
            }

            if (!string.Equals(table.Code, request.Code, StringComparison.OrdinalIgnoreCase))
            {
                var codeExists = await _tableRepository.ExistsByCodeAsync(request.Code);
                if (codeExists)
                    throw new ConflictException($"كود الطاولة '{request.Code}' مستخدم من قبل طاولة أخرى.");
            }

            _mapper.Map(request, table);
            table.UpdatedAt = DateTime.UtcNow;

            await _tableRepository.UpdateAsync(table);

            await SafeLogActivityAsync(new CreateActivityLogDto
            {
                UserId = null,
                UserName = "System",
                UserRole = "System",
                ActionType = ActivityActionType.TableUpdated,
                Module = "Tables",
                EntityName = nameof(Table),
                EntityId = table.Id,
                Description = $"Updated table '{table.TableNumber}' with code '{table.Code}'.",
                OldValue = oldValue,
                NewValue = BuildTableValue(table)
            });

            return _mapper.Map<TableResponseDto>(table);
        }

        // 6. حذف طاولة Soft Delete
        public async Task DeleteTableAsync(Guid id)
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
                throw new NotFoundException("الطاولة", id);

            var oldValue = BuildTableValue(table);

            await _tableRepository.DeleteAsync(id);

            await SafeLogActivityAsync(new CreateActivityLogDto
            {
                UserId = null,
                UserName = "System",
                UserRole = "System",
                ActionType = ActivityActionType.TableDeleted,
                Module = "Tables",
                EntityName = nameof(Table),
                EntityId = table.Id,
                Description = $"Deleted table '{table.TableNumber}' with code '{table.Code}'.",
                OldValue = oldValue
            });
        }

        private static void NormalizeCreateRequest(CreateTableRequestDto request)
        {
            request.TableNumber = request.TableNumber.Trim();
            request.Code = request.Code.Trim();
            request.Location = request.Location.Trim();
            request.Zone = string.IsNullOrWhiteSpace(request.Zone) ? null : request.Zone.Trim();
        }

        private static void NormalizeUpdateRequest(UpdateTableRequestDto request)
        {
            request.TableNumber = request.TableNumber.Trim();
            request.Code = request.Code.Trim();
            request.Location = request.Location.Trim();
            request.Zone = string.IsNullOrWhiteSpace(request.Zone) ? null : request.Zone.Trim();
        }

        private async Task SafeLogActivityAsync(CreateActivityLogDto dto)
        {
            try
            {
                await _activityLogService.LogAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "⚠️ Failed to write activity log. Module: {Module}, Action: {ActionType}, Entity: {EntityName}, EntityId: {EntityId}",
                    dto.Module,
                    dto.ActionType,
                    dto.EntityName,
                    dto.EntityId);
            }
        }

        private static string BuildTableValue(Table table)
        {
            return
                $"TableNumber={table.TableNumber}; " +
                $"Code={table.Code}; " +
                $"Capacity={table.Capacity}; " +
                $"Location={table.Location}; " +
                $"Zone={table.Zone}; " +
                $"FloorNumber={table.FloorNumber}; " +
                $"Status={table.Status}; " +
                $"IsActive={table.IsActive}; " +
                $"IsOrderingEnabled={table.IsOrderingEnabled}";
        }
    }
}
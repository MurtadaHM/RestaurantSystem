using AutoMapper;
using RestaurantSystem.Application.Contracts.Repositories;
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
        private readonly IMapper _mapper;

        public TableService(ITableRepository tableRepository, IMapper mapper)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
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

            return _mapper.Map<TableResponseDto>(table);
        }

        // 5. تحديث بيانات طاولة موجودة
        public async Task<TableResponseDto> UpdateTableAsync(Guid id, UpdateTableRequestDto request)
        {
            NormalizeUpdateRequest(request);

            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
                throw new NotFoundException("الطاولة", id);

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

            return _mapper.Map<TableResponseDto>(table);
        }

        // 6. حذف طاولة (Soft Delete)
        public async Task DeleteTableAsync(Guid id)
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
                throw new NotFoundException("الطاولة", id);

            await _tableRepository.DeleteAsync(id);
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
    }
}
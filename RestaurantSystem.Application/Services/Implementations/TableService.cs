using AutoMapper;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Tables;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Exceptions;
using RestaurantSystem.Domain.Enums;

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
            var table = await _tableRepository.GetByIdAsync(id);

            if (table == null)
                throw new NotFoundException("الطاولة", id);

            return _mapper.Map<TableResponseDto>(table);
        }

        // 4. إنشاء طاولة جديدة
        public async Task<TableResponseDto> CreateTableAsync(CreateTableRequestDto request)
        {
            // التأكد من أن رقم الطاولة غير مكرر
            var exists = await _tableRepository.ExistsByTableNumberAsync(request.TableNumber);
            if (exists)
                throw new ConflictException($"رقم الطاولة '{request.TableNumber}' موجود بالفعل.");

            var table = _mapper.Map<Table>(request);

            // الحالة الافتراضية عند الإنشاء
            table.Status = TableStatus.Available;
            table.CreatedAt = DateTime.UtcNow;

            await _tableRepository.AddAsync(table);

            return _mapper.Map<TableResponseDto>(table);
        }

        // 5. تحديث بيانات طاولة موجودة
        public async Task<TableResponseDto> UpdateTableAsync(Guid id, UpdateTableRequestDto request)
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
                throw new NotFoundException("الطاولة", id);

            // إذا تغير رقم الطاولة، نتأكد أنه غير مستخدم في طاولة أخرى
            if (!string.Equals(table.TableNumber, request.TableNumber, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _tableRepository.ExistsByTableNumberAsync(request.TableNumber);
                if (exists)
                    throw new ConflictException($"رقم الطاولة '{request.TableNumber}' مستخدم من قبل طاولة أخرى.");
            }

            // تحديث البيانات باستخدام AutoMapper
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

            // ملاحظة: الـ Repository يتولى عملية الـ Soft Delete بناءً على إعدادات الـ SaveChanges التي كتبناها
            await _tableRepository.DeleteAsync(id);
        }
    }
}
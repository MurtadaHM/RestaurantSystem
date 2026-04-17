using AutoMapper;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Departments;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Domain.Exceptions;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IMapper mapper,
            ILogger<DepartmentService> logger)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<DepartmentResponseDto>> GetAllDepartmentsAsync()
        {
            _logger.LogInformation("Fetching all departments");
            var departments = await _departmentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DepartmentResponseDto>>(departments);
        }

        public async Task<IEnumerable<DepartmentResponseDto>> GetActiveDepartmentsAsync()
        {
            _logger.LogInformation("Fetching active departments");
            var departments = await _departmentRepository.GetActiveDepartmentsAsync();
            return _mapper.Map<IEnumerable<DepartmentResponseDto>>(departments);
        }

        public async Task<DepartmentResponseDto> GetDepartmentByIdAsync(Guid id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                throw new NotFoundException("Department", id);

            return _mapper.Map<DepartmentResponseDto>(department);
        }

        public async Task<DepartmentResponseDto> CreateDepartmentAsync(CreateDepartmentRequestDto request)
        {
            // 🛡️ التحقق من عدم تكرار الاسم (Business Rule)
            var allDepartments = await _departmentRepository.GetAllAsync();
            if (allDepartments.Any(d => d.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Attempt to create duplicate department: {Name}", request.Name);
                throw new ConflictException($"القسم '{request.Name}' موجود بالفعل في النظام.");
            }

            var department = _mapper.Map<Department>(request);
            department.CreatedAt = DateTime.UtcNow;

            await _departmentRepository.AddAsync(department);
            _logger.LogInformation("Department created successfully: {DepartmentName}", department.Name);

            return _mapper.Map<DepartmentResponseDto>(department);
        }

        public async Task<DepartmentResponseDto> UpdateDepartmentAsync(Guid id, UpdateDepartmentRequestDto request)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) throw new NotFoundException("Department", id);

            // تحديث البيانات
            _mapper.Map(request, department);
            department.UpdatedAt = DateTime.UtcNow;

            await _departmentRepository.UpdateAsync(department);
            _logger.LogInformation("Department updated: {DepartmentId}", id);

            return _mapper.Map<DepartmentResponseDto>(department);
        }

        public async Task<bool> UpdateStatusAsync(Guid id, DepartmentStatus status)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) throw new NotFoundException("Department", id);

            department.Status = status;
            department.UpdatedAt = DateTime.UtcNow;

            await _departmentRepository.UpdateAsync(department);
            _logger.LogInformation("Status updated for department {DepartmentId} to {Status}", id, status);
            return true;
        }

        public async Task<bool> DeleteDepartmentAsync(Guid id)
        {
            // 🛡️ التحقق من وجود القسم ومن الأصناف المرتبطة
            var department = await _departmentRepository.GetWithMenuItemsAsync(id);
            if (department == null) throw new NotFoundException("Department", id);

            if (department.MenuItems.Any())
            {
                _logger.LogWarning("Failed delete attempt: Department {Id} has linked menu items", id);
                throw new ConflictException("لا يمكن حذف القسم لأنه يحتوي على أصناف منيو مرتبطة به.");
            }

            // ✅ التصحيح هنا: نمرر الـ id وليس الكائن department
            await _departmentRepository.DeleteAsync(id);

            _logger.LogInformation("Department deleted: {Id}", id);
            return true;
        }
    }
}
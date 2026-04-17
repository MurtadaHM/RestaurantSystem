using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantSystem.Application.DTOs.Auth;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<UserListItemDto> CreateStaffAsync(CreateStaffRequestDto request);
        Task<IEnumerable<UserListItemDto>> GetAllUsersAsync();
        Task<IEnumerable<UserListItemDto>> GetUsersByRoleAsync(string role);
        Task<UserListItemDto> UpdateUserRoleAsync(Guid userId, UpdateUserRoleRequestDto request);
        Task<bool> ToggleUserStatusAsync(Guid userId, ToggleUserStatusRequestDto request);
    }
}
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Contracts.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByPhoneNumberAsync(string phoneNumber);

        Task AddAsync(Customer customer);

        Task SaveChangesAsync();
    }
}
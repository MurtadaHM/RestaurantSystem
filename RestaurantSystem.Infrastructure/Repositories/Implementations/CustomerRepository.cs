using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Infrastructure.Data;

namespace RestaurantSystem.Infrastructure.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Customer> _dbSet;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Customer>();
        }

        public async Task<Customer?> GetByPhoneNumberAsync(string phoneNumber)
        {
            var normalized = phoneNumber?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                return null;

            return await _dbSet.FirstOrDefaultAsync(c => c.PhoneNumber == normalized);
        }

        public async Task AddAsync(Customer customer)
        {
            await _dbSet.AddAsync(customer);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
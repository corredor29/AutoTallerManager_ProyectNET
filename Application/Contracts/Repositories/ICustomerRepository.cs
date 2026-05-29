using Domain.Entities.Customers;

namespace Application.Contracts.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        void Update(Customer customer);
        void Remove(Customer customer);
    }
}

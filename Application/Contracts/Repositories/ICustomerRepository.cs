using Application.Common.Pagination;
using Application.Filters;
using Domain.Entities.Customers;

namespace Application.Contracts.Repositories
{
    public interface ICustomerRepository
    {
        Task<PagedResult<Customer>> GetAllPagedAsync(PaginationParams pagination, CustomerFilter filter);
        Task<Customer?> GetByIdAsync(int id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        void Update(Customer customer);
        void Remove(Customer customer);
    }
}

using Application.Common.Pagination;
using Application.DTOs.Customers;
using Application.Requests.Customers;

namespace Application.Contracts.Services
{
    public interface ICustomerService
    {
        Task<PagedResult<CustomerDto>> GetAllAsync(GetCustomersRequest request);
        Task<CustomerDto?> GetByIdAsync(int id);
        Task<CustomerDto> CreateAsync(CreateCustomerRequest request);
        Task<bool> UpdateAsync(int id, UpdateCustomerRequest request);
        Task<bool> DeleteAsync(int id);
    }
}

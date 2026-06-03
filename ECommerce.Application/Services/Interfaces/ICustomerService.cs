using ECommerce.Application.DTOs;

namespace ECommerce.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto?> GetByIdAsync(Guid id);
        Task<CustomerDto> AddAsync(CustomerDto dto);
        Task UpdateAsync(CustomerDto dto);
        Task DeleteAsync(Guid id);
    }
}

using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber
            });
        }

        public async Task<CustomerDto?> GetByIdAsync(Guid id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return null;

            return new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber
            };
        }

        public async Task<CustomerDto> AddAsync(CustomerDto dto)
        {
            var entity = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            await _repository.AddAsync(entity);
            dto.Id = entity.Id;
            return dto;
        }

        public async Task UpdateAsync(CustomerDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null) return;

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            entity.PhoneNumber = dto.PhoneNumber;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);
    }
}

using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CustomerService
    {
        private readonly IRepository<Customer> _customer;
        private readonly IMapper _mapper;

        public CustomerService(IRepository<Customer> customer, IMapper mapper)
        {
            _customer = customer;
            _mapper = mapper;
        }

        public async Task<List<CustomerDTO>> GetCustomersAsync()
        {
            var customers = await _customer.GetAllAsync();
            return _mapper.Map<List<CustomerDTO>>(customers);
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(int id)
        {
            var customers = await _customer.GetByIdAsync(id);
            return _mapper.Map<CustomerDTO>(customers);
        }

        public async Task<ServiceResponse> CreateCustomerAsync(CustomerDTO customerDTO)
        {
            var newCustomer = _mapper.Map<Customer>(customerDTO);
            await _customer.AddAsync(newCustomer);
            return new ServiceResponse(true, "Created");
        }

        public async Task<ServiceResponse> UpdateCustomerAsync(int customerId, CustomerDTO customerDTO)
        {
            var existingCustomer = await _customer.GetByIdAsync(customerId);

            if (existingCustomer != null)
            {
                existingCustomer.Name = customerDTO.Name;
                existingCustomer.Age = customerDTO.Age;
                existingCustomer.Address = customerDTO.Address;

                await _customer.UpdateAsync(existingCustomer);
                return new ServiceResponse(true, "Updated");
            }
            return new ServiceResponse(false, "Customer not found");
        }

        public async Task<ServiceResponse> DeleteCustomerAsync(int customerId)
        {
            await _customer.DeleteAsync(customerId);
            return new ServiceResponse(true, "Deleted");
        }
    }
}

using Application.DTOs;
using Domain.Entities;
using System.Net.Http.Json;

namespace WebUI.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerDTO>> GetCustomers();
        Task<CustomerDTO> GetCustomerById(int customerId);
        Task<ServiceResponse> AddCustomer(CustomerDTO customer);
        Task<ServiceResponse> UpdateCustomer(int customerId, CustomerDTO customer);
        Task<ServiceResponse> DeleteCustomer(int customerId);
    }

    public class CustomerService : ICustomerService
    {

        private readonly HttpClient _httpClient;

        public CustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CustomerDTO>> GetCustomers() => 
            await _httpClient.GetFromJsonAsync<List<CustomerDTO>>("api/Customer")!;

        public async Task<CustomerDTO> GetCustomerById(int customerId) =>
            await _httpClient.GetFromJsonAsync<CustomerDTO>($"api/customer/{customerId}");

        public async Task<ServiceResponse> AddCustomer(CustomerDTO customer)
        {
            var data = await _httpClient.PostAsJsonAsync("api/customer", customer);
            return await data.Content.ReadFromJsonAsync<ServiceResponse>();
        }

        public async Task<ServiceResponse> UpdateCustomer(int customerId, CustomerDTO customer)
        {
            var data = await _httpClient.PutAsJsonAsync($"api/customer/{customerId}", customer);
            return await data.Content.ReadFromJsonAsync<ServiceResponse>();
        }

        public async Task<ServiceResponse> DeleteCustomer(int customerId)
        {
            var data = await _httpClient.DeleteAsync($"api/customer/{customerId}");
            return await data.Content.ReadFromJsonAsync<ServiceResponse>();
        }
    }
}

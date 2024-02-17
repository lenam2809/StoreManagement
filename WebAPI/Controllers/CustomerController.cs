using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerDTO>>> GetCustomers()
        {
            var customers = await _customerService.GetCustomersAsync();
            return Ok(customers);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomerById(int id)
        {
            var data = await _customerService.GetCustomerByIdAsync(id);
            return Ok(data);
        }

        // POST api/<CustomerController>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CustomerDTO value)
        {
            var result = await _customerService.CreateCustomerAsync(value);
            return Ok(result);
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerDTO value)
        {
            var result = await _customerService.UpdateCustomerAsync(id,value);
            return Ok(result);
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _customerService.DeleteCustomerAsync(id);
            return Ok(data);
        }
    }
}

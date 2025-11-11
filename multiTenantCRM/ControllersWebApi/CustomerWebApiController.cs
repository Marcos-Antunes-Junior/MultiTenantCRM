using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using multiTenantCRM.Data;
using multiTenantCRM.Models;

namespace multiTenantCRM.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerWebApiController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public CustomerWebApiController(CrmDbContext context)
        {
            _context = context;
        }

        // GET: api/CustomerWebApi/{tenantId}
        [HttpGet("{tenantId:guid}")]
        public async Task<IActionResult> GetCustomers(Guid tenantId)
        {
            var customers = await _context.Customers
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();

            return Ok(customers);
        }

        // GET: api/CustomerWebApi/{tenantId}/{id}
        [HttpGet("{tenantId:guid}/{id:int}")]
        public async Task<IActionResult> GetCustomer(Guid tenantId, int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        // POST: api/CustomerWebApi/{tenantId}
        [HttpPost("{tenantId:guid}")]
        public async Task<IActionResult> CreateCustomer(Guid tenantId, [FromBody] Customer customer)
        {
            if (customer == null)
                return BadRequest("Customer data is required.");

            customer.TenantId = tenantId;
            customer.CreatedAt = DateTime.UtcNow;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { tenantId, id = customer.Id }, customer);
        }

        // PUT: api/CustomerWebApi/{tenantId}/{id}
        [HttpPut("{tenantId:guid}/{id:int}")]
        public async Task<IActionResult> UpdateCustomer(Guid tenantId, int id, [FromBody] Customer customer)
        {
            var existing = await _context.Customers
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == id);

            if (existing == null)
                return NotFound();

            existing.Name = customer.Name;
            existing.Email = customer.Email;
            existing.Phone = customer.Phone;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

       
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using multiTenantCRM.Data;
using multiTenantCRM.Models;
using multiTenantCRM.Services;
using Npgsql;

namespace multiTenantCRM.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerWebApiController : ControllerBase
    {
        private readonly CrmDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public CustomerWebApiController(CrmDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        // GET: api/CustomerWebApi
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            Guid tenantId = _tenantProvider.TenantId;
            var customers = await _context.Customers
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();

            return Ok(customers);
        }

        // GET: api/CustomerWebApi/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            Guid tenantId = _tenantProvider.TenantId;
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        // POST: api/CustomerWebApi/       
        [HttpPost()]
        public async Task<IActionResult> CreateCustomer(Guid tenantId, [FromBody] Customer customer)
        {
            if (customer == null)
                return BadRequest("Customer data is required.");

            customer.TenantId = _tenantProvider.TenantId;
            customer.CreatedAt = DateTime.UtcNow;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { tenantId, id = customer.Id }, customer);
        }

        // PUT: api/CustomerWebApi/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer( int id, [FromBody] Customer customer)
        {
            Guid tenantId = _tenantProvider.TenantId;
            var existing = await _context.Customers
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == id);

            if (existing == null)
                return NotFound();

            existing.Name = customer.Name;
            existing.Email = customer.Email;
            existing.Phone = customer.Phone;
            existing.IsActive = customer.IsActive;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpGet("GetCustomerAndTenantData")]
        public async Task<List<GetCustomerTenantData>> GetCustomerAndTenantData()
        {
            try
            {
                var sql = @"
                    SELECT 
                        c.""Id"" AS CustomerId,
                        c.""Name"" AS CustomerName,
                        c.""Email"" AS Email,
                        c.""TenantId"" AS TenantId,
                        t.""Name"" AS TenantName,
                        t.""Domain"" AS TenantEmail

                    FROM ""Customers"" c
                    INNER JOIN ""Tenants"" t 
                        ON t.""Id"" = c.""TenantId""
                    WHERE c.""TenantId"" = @tenantId
                ";

                Guid tenantId = _tenantProvider.TenantId;

                var result = await _context.Database
                    .SqlQueryRaw<GetCustomerTenantData>(sql, new NpgsqlParameter("tenantId", tenantId))
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

       
    }
}

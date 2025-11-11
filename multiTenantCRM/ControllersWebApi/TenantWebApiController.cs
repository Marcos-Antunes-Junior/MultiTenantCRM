using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using multiTenantCRM.Data;
using multiTenantCRM.Models;
using multiTenantCRM.Utils;


namespace multiTenantCRM.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantWebApiController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public TenantWebApiController(CrmDbContext context)
        {
            _context = context;
        }

        // GET: api/TenantWebApi
        [HttpGet]
        public async Task<IActionResult> GetTenants()
        {
            var tenants = await _context.Tenants.ToListAsync();
            return Ok(tenants);
        }

        // GET: api/TenantWebApi/status
        [HttpGet("status")]
        public async Task<IActionResult> GetTenantsStatus()
        {
            // Load all tenants
            var tenants = await _context.Tenants.ToListAsync();

            // ✅ Always initialize the list before using it
            var tenantStatusList = new List<TenantStatus>();

            // ✅ Loop through tenants (only if any exist)
            if (tenants != null && tenants.Count > 0)
            {
                foreach (var tenant in tenants)
                {
                    tenantStatusList.Add(new TenantStatus
                    {
                        Name = tenant.Name,
                        Status = tenant.IsActive
                    });
                }
            }

            // ✅ Return OK with empty list if no tenants (safe for frontend)
            return Ok(tenantStatusList);
        }

        // GET: api/TenantWebApi/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTenant(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);

            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        // POST: api/TenantWebApi
        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] Tenant tenant)
        {
            if (tenant == null)
                return BadRequest("Tenant data is required.");

            tenant.Id = Guid.NewGuid();
            tenant.CreatedAt = DateTime.UtcNow;

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
            FileLogger.LogTenantCreation(tenant.Name);


            return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
        }

        // PUT: api/TenantWebApi/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] Tenant updated)
        {
            var tenant = await _context.Tenants.FindAsync(id);

            if (tenant == null)
                return NotFound();

            tenant.Name = updated.Name;
            tenant.Domain = updated.Domain;
            tenant.IsActive = updated.IsActive;

            await _context.SaveChangesAsync();
            return Ok(tenant);
        }

        // DELETE: api/TenantWebApi/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);

            if (tenant == null)
                return NotFound();

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

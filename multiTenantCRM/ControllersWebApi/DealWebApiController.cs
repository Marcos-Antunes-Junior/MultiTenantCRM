using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using multiTenantCRM.Data;
using multiTenantCRM.Models;

namespace multiTenantCRM.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealWebApiController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public DealWebApiController(CrmDbContext context)
        {
            _context = context;
        }

        // GET: api/DealWebApi/{tenantId}
        [HttpGet("{tenantId:guid}")]
        public async Task<IActionResult> GetDeals(Guid tenantId)
        {
            var deals = await _context.Deals
                .Where(d => d.TenantId == tenantId)
                .Include(d => d.Customer)
                .ToListAsync();

            return Ok(deals);
        }

        // POST: api/DealWebApi/{tenantId}
        [HttpPost("{tenantId:guid}")]
        public async Task<IActionResult> CreateDeal(Guid tenantId, [FromBody] Deal deal)
        {
            if (deal == null)
                return BadRequest("Deal data is required.");

            // Always enforce the TenantId from the route
            deal.TenantId = tenantId;
            deal.CreatedAt = DateTime.UtcNow;

            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDeals), new { tenantId = deal.TenantId }, deal);
        }
    }
}

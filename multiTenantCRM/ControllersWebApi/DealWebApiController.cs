using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using multiTenantCRM.Data;
using multiTenantCRM.Models;

namespace multiTenantCRM.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DealWebApiController : ControllerBase
    {
        private readonly CrmDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public DealWebApiController(CrmDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        // GET: api/DealWebAp
        [HttpGet]
        public async Task<IActionResult> GetDeals()
        {
            Guid tenantId = _tenantProvider.TenantId;
            var deals = await _context.Deals
                .Where(d => d.TenantId == tenantId)
                .Include(d => d.Customer)
                .ToListAsync();

            return Ok(deals);
        }

        // POST: api/DealWebApi}
       [HttpPost]
        public async Task<IActionResult> CreateDeal([FromBody] CreateDealDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deal = new Deal
            {
                TenantId = _tenantProvider.TenantId,
                CustomerId = dto.CustomerId,
                Title = dto.Title,
                Value = dto.Value,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();

            return Ok(deal);
        }

        // DELETE: api/DealWebApi
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDealById(int id)
        {
            Guid tenantId = _tenantProvider.TenantId;
            var deal = await _context.Deals
              .FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId);

            if (deal == null)
                return NotFound();

            _context.Deals.Remove(deal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

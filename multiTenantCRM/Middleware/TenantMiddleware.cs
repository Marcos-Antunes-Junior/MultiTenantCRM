using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using multiTenantCRM.Services;

namespace multiTenantCRM.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
        {
            if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantIdValue)
                && Guid.TryParse(tenantIdValue, out var tenantId))
            {
                tenantProvider.SetTenant(tenantId);
            }

            await _next(context);
        }
    }
}

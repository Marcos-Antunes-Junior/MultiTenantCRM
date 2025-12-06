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

        var path = context.Request.Path.Value?.ToLower();

        // Skip tenant requirement for normal web pages (MVC)
        if (!path.StartsWith("/api"))
        {
            await _next(context);
            return;
        }

        var headerTenant = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();

        if (Guid.TryParse(headerTenant, out Guid tenantId))
        {
            tenantProvider.SetTenant(tenantId);
        }
        else
        {
            // No tenant provided → reject or default
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Missing or invalid X-Tenant-ID header.");
            return;
        }

        await _next(context);
    }
}
}



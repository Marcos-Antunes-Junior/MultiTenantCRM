using multiTenantCRM.Models;

namespace multiTenantCRM.Services
{
    public interface ITokenService
    {
        string GenerateToken(Guid tenantId, Int32 userId, string email);
    }
}

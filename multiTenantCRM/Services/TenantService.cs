namespace multiTenantCRM.Services
{
     public interface ITenantProvider
    {
        Guid TenantId { get; }
        void SetTenant(Guid tenantId);
    }

     public class TenantProvider : ITenantProvider
    {
        public Guid TenantId { get; private set; }

        public void SetTenant(Guid tenantId)
        {
            TenantId = tenantId;
        }
    }
}
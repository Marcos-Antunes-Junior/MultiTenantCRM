public interface ITenantProvider
{
    Guid TenantId { get; }
    void SetTenant(Guid id);
}

public class TenantProvider : ITenantProvider
{
    private Guid _tenantId;
    public Guid TenantId => _tenantId;

    public void SetTenant(Guid id)
    {
        _tenantId = id;
    }
}

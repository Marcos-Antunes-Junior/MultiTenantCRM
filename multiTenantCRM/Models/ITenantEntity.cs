using System;

namespace multiTenantCRM.Models
{
    public interface ITenantEntity
    {
        Guid TenantId { get; set; }
    }
}

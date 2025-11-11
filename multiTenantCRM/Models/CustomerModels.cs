using System;
using System.Collections.Generic;

namespace multiTenantCRM.Models
{
    public class Customer : ITenantEntity
    {
        public Int32 Id { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Deal>? Deals { get; set; }
    }

}
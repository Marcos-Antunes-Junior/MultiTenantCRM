using System;
using System.Collections.Generic;

namespace multiTenantCRM.Models
{
    public class Deal : ITenantEntity
    {
        public Int32 Id { get; set; }
        public Guid TenantId { get; set; }
        public int CustomerId { get; set; }
        public  Customer Customer { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Status { get; set; } = "Open";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CreateDealDto
    {
        public int CustomerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Status { get; set; } = "Open";
    }


}
using System;
using System.Collections.Generic;

namespace multiTenantCRM.Models
{
    public class Customer : ITenantEntity
    {
        public Int32 Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class GetCustomerTenantData
    {
        public int CustomerId {get; set;}
        public Guid TenantId {get; set;}
        public string CustomerName {get; set;}
        public string Email {get; set;}
        public string TenantName {get; set;}
        public string TenantEmail {get; set;}

    }

}
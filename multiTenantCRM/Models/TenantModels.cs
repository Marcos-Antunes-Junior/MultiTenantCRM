using System;
using System.Collections.Generic;

namespace multiTenantCRM.Models
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Domain { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }
        public ICollection<User>? Users { get; set; }
        public ICollection<Customer>? Customers { get; set; }
    }

    public struct TenantStatus
    {
        public string Name;
        public bool Status;

        public TenantStatus(string name, bool status)
        {
            Name = name;
            Status = status;
        }

        public override string ToString()
        {
            return $"Tenant name: {Name} - Status: {(Status ? "Active" : "Inactive")}";
        }
    }



}
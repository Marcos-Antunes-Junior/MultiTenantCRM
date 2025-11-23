namespace multiTenantCRM.Models.Dtos
{
    public class LoginDto
    {
        public Guid TenantId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

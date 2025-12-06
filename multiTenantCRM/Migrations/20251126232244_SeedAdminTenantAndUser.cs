using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace multiTenantCRM.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminTenantAndUser : Migration
    {
        /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
    {
        var adminTenantId = Guid.NewGuid();
        var adminUserId = Guid.NewGuid();

        // BCrypt hash for password: Admin123!
        // Replace with your real hashed password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");


        migrationBuilder.Sql($@"
            INSERT INTO ""Tenants"" (""Id"", ""Name"", ""Domain"", ""CreatedAt"", ""IsActive"")
            VALUES ('{adminTenantId}', 'ADMIN', 'admin@system.local', NOW(), true);

            INSERT INTO ""Users"" 
                (""TenantId"", ""FullName"", ""Email"", ""PasswordHash"", ""CreatedAt"", ""IsActive"", ""Role"")
            VALUES 
                ('{adminTenantId}', 'System Administrator', 'admin@system.local', '{passwordHash}', NOW(), true, 'ADMIN');
        ");
    }

        /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            DELETE FROM ""Users"" WHERE ""Email"" = 'admin@system.local';
            DELETE FROM ""Tenants"" WHERE ""Name"" = 'ADMIN';
        ");
    }
    }
}

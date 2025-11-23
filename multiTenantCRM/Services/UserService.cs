using Microsoft.EntityFrameworkCore;
using multiTenantCRM.Models;
using multiTenantCRM.Data;
using BCrypt.Net;

public class UserService
{
    private readonly CrmDbContext _db;

    public UserService(CrmDbContext db)
    {
        _db = db;
    }

    public async Task<User> CreateUserAsync(CreateUserDto dto)
    {
        dto.Email = dto.Email.Trim().ToLower();


        var exists = await _db.Users
            .AnyAsync(u => u.Email == dto.Email && u.Tenant.Id == dto.Tenant);


        if (exists)
        {
            throw new Exception("A user with this email already exists in this tenant.");
        }

        var getTenant = await _db.Tenants.FirstOrDefaultAsync(u => u.Id == dto.Tenant);

        if(getTenant == null)
        {
            throw new Exception("Company not found.");
        }
                
        var user = new User
        {
            Tenant = getTenant,
            FullName = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user;
    }
}

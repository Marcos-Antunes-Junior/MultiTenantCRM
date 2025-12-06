using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using multiTenantCRM.Data;
using multiTenantCRM.Models;
using multiTenantCRM.Models.Dtos;
using multiTenantCRM.Services;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
   private readonly UserService _service;
    private readonly CrmDbContext _context;
    private readonly ITokenService _tokenService;


    public UsersController(
        UserService service,
        CrmDbContext context,
        ITokenService tokenService)
    {
        _service = service;
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        try
        {
            var user = await _service.CreateUserAsync(dto);

            return Ok(new
            {
                message = "User created successfully",
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Tenant
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.TenantId == login.TenantId &&
                    u.Email == login.Email);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }
                


            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }
    

            string token = _tokenService.GenerateToken(
                user.TenantId,
                user.Id,
                user.Email
            );

            return Ok(new
            {
                token,
                user = new { user.Id, user.Email, user.TenantId }
            });
        }
}

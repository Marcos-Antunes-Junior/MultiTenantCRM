using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using multiTenantCRM.Data;
using multiTenantCRM.Services;


public class AccountController : Controller
{
    private readonly CrmDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ITenantProvider _tenantProvider;

       public AccountController(
        CrmDbContext context,
        ITokenService tokenService,
        ITenantProvider tenantProvider
        )
    {
        _context = context;
        _tokenService = tokenService;
        _tenantProvider = tenantProvider;
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

   
[HttpPost]
[AllowAnonymous]
public async Task<IActionResult> Login(string Email, string Password, string Token)
{
    var tenant = await _context.Tenants
        .FirstOrDefaultAsync(t => t.Token == Token);

    if (tenant == null)
    {
        ViewBag.Error = "Invalid tenant token";
        return View();
    }

    var getUser = await _context.Users
        .IgnoreQueryFilters()     // IMPORTANT
        .FirstOrDefaultAsync(u => u.Email == Email && u.TenantId == tenant.Id);

    if (getUser == null)
    {
        ViewBag.Error = "Invalid login";
        return View();
    }

    if (!BCrypt.Net.BCrypt.Verify(Password, getUser.PasswordHash))
    {
        ViewBag.Error = "Invalid credentials";
        return View();
    }

    // Create claims
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, getUser.FullName),
        new Claim(ClaimTypes.NameIdentifier, getUser.Id.ToString()),
        new Claim(ClaimTypes.Email, getUser.Email),
        new Claim("TenantId", getUser.TenantId.ToString())
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    // Create authentication cookie
    var authProps = new AuthenticationProperties
    {
        IsPersistent = true,
        ExpiresUtc = DateTime.UtcNow.AddHours(12)
    };

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        authProps);

    return RedirectToAction("Index", "Home");
}

[HttpPost]
public async Task<IActionResult> Logout()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToAction("Index", "Home"); 
}


}



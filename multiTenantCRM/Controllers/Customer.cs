using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using multiTenantCRM.Data;
using multiTenantCRM.Models;

public class CustomerController : Controller
{
    private readonly CrmDbContext _context;
    private readonly ITenantProvider _tenantProvider;

    public CustomerController(CrmDbContext context, ITenantProvider tenantProvider)
    {
        _context = context;
        _tenantProvider = tenantProvider;
    }

    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");

        var tenantId = Guid.Parse(User.Claims.First(c => c.Type == "TenantId").Value);

        var customers = await _context.Customers
            .IgnoreQueryFilters()
            .Where(c => c.TenantId == tenantId)
            .ToListAsync();

        return View(customers);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");

        var tenantId = Guid.Parse(User.Claims.First(c => c.Type == "TenantId").Value);

        var customer = await _context.Customers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (customer == null)
            return NotFound();

        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Customer model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var tenantId = Guid.Parse(User.Claims.First(c => c.Type == "TenantId").Value);

        var customer = await _context.Customers
        .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (customer == null)
            return NotFound();

        customer.Name = model.Name;
        customer.Email = model.Email;
        customer.Phone = model.Phone;
        customer.IsActive = true;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Customer");
    }

    // GET: /Customer/Create
    public IActionResult Create()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");

        return View();
    }

    // POST: /Customer/Create
    [HttpPost]
    public async Task<IActionResult> Create(Customer model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Get TenantId from claims
        var tenantId = Guid.Parse(User.Claims.First(c => c.Type == "TenantId").Value);
        model.TenantId = tenantId;
        model.CreatedAt = DateTime.UtcNow;
        model.IsActive = true;

        _context.Customers.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");

        var tenantId = Guid.Parse(User.Claims.First(c => c.Type == "TenantId").Value);

        var customer = await _context.Customers
        .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (customer == null)
            return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }



}

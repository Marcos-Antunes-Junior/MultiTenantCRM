using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous] // Allow access for redirect logic
public class HomeController : Controller
{
    public IActionResult Index()
    {
       if (!User.Identity.IsAuthenticated)
       {
         return RedirectToAction("Login", "Account");
        }
  
        return View();

    }
     

}
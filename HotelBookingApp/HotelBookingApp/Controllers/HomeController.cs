using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class HomeController: Controller
{

    public IActionResult Index()
    {
        var token = HttpContext.Session.GetString("jwtToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login");
        }

        return View();
    }

}
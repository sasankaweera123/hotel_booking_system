using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class HomeController: Controller
{

    public IActionResult Index() => View();

}
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class LoginController: Controller
{
    private readonly AuthService _authService;

    public LoginController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string username, string password)
    {
        bool loggedIn = await _authService.LoginAsync(username, password);
        if (loggedIn)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Invalid username or password";
        return View();
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string password, string role)
    {
        bool registered = await _authService.RegisterAsync(username, password, role);

        if (registered)
        {
            // After registration, redirect to login
            return RedirectToAction("Index", "Login");
        }

        ViewBag.Error = "Registration failed. Try a different username.";
        return View();
    }

    [HttpGet]
    public IActionResult Logout()
    {
        _authService.Logout();
        return RedirectToAction("Index");
    }
}
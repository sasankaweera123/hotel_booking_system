using HotelBookingApp.Models.Utils;
using HotelBookingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApp.Controllers;

public class LoginController(AuthService authService, LogQueue logQueue) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string username, string password)
    {
        var loggedIn = await authService.LoginAsync(username, password);
        if (loggedIn)
        {
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = "User logged in successfully",
            });
            return RedirectToAction("Index", "Home");
        }
        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "ERROR",
            Message = "Login failed for user: " + username,
        });

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
        var registered = await authService.RegisterAsync(username, password, role);

        if (registered)
        {
            logQueue.Queue.Enqueue(new LogMessage
            {
                Level = "INFO",
                Message = "User registered successfully: " + username,
            });
            return RedirectToAction("Index", "Login");
        }
        
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "ERROR",
            Message = "Registration failed for user: " + username,
        });

        ViewBag.Error = "Registration failed. Try a different username.";
        return View();
    }

    [HttpGet]
    public IActionResult Logout()
    {
        logQueue.Queue.Enqueue(new LogMessage
        {
            Level = "INFO",
            Message = "User logged out successfully",
        });
        authService.Logout();
        return RedirectToAction("Index");
    }
}
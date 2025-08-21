using HotelBookingApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// 1. Add MVC with views
builder.Services.AddControllersWithViews();

// 2. Add HttpClient to call API Gateway
builder.Services.AddHttpClient("ApiClient", client =>
    {
        var dev = "https://localhost:6000";
        var prod = "https://hotelapp.hivesphere.world";
        client.BaseAddress = new Uri(prod);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            // WARNING: ONLY for development/testing, bypass SSL cert validation
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    });


// 3. Add HttpContextAccessor & Session to store JWT
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // where to redirect if not authenticated
        options.AccessDeniedPath = "/AccessDenied"; // optional
    });
// 4. Add application services
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ChatBotService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<LogQueue>();
builder.Services.AddHostedService<FileLoggerService>();

var app = builder.Build();

// 5. Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // for CSS/JS

app.UseRouting();
app.UseSession();      // enable session for JWT storage
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
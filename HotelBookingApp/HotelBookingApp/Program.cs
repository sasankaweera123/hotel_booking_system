using HotelBookingApp.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add MVC with views
builder.Services.AddControllersWithViews();

// 2. Add HttpClient to call API Gateway
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:6000"); // Gateway base URL
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

// 4. Add application services
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ChatBotService>();
builder.Services.AddScoped<AuthService>();

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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
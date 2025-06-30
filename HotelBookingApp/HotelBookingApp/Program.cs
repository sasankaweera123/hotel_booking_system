using HotelBookingApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<BookingService>();
builder.Services.AddSingleton<RoomService>();
builder.Services.AddSingleton<RequestService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Booking}/{action=Index}/{id?}");


app.Run();
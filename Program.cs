using opdracht_1.Models;
using opdracht_1.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DierentuinDB service
builder.Services.AddScoped<ZooData>(); // Assuming DierentuinDB is scoped service, adjust it as per your requirement

// Configure session
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Session";
    options.IdleTimeout = TimeSpan.FromDays(1); // Adjust as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Add session middleware before MVC
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Add session middleware before MVC
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "addTickets",
    pattern: "AddTickets",
    defaults: new { controller = "Tickets", action = "AddTickets" });

app.MapControllerRoute(
    name: "register",
    pattern: "Register",
    defaults: new { controller = "Account", action = "Register" });

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ZooData>();

    // Controleer of het 14:30 uur is
    var currentTime = DateTime.Now;
    var purgeTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 14, 30, 0);

    if (currentTime.Hour == 14 && currentTime.Minute == 30)
    {
        // Verwijder de inhoud van de Orders-tabel
        var orders = context.Orders;
        context.Orders.RemoveRange(orders);

        // Verwijder de inhoud van de OrderDetails-tabel
        var orderDetails = context.OrderDetails;
        context.OrderDetails.RemoveRange(orderDetails);

        context.SaveChanges(); // Apply database changes
    }
}

app.Run();
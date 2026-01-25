using CookiesAuth.Configuration;
using CookiesAuth.Models;
using CookiesAuth.Services;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);   

// bind DataProtection settings to strongly-typed POCO and register
builder.Services.Configure<CookiesAuth.Configuration.DataProtectionOptions>(builder.Configuration.GetSection("DataProtection"));

// register the protector wrapper service
builder.Services.AddSingleton<IUserDataProtector, UserDataProtector>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<UserStoreService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

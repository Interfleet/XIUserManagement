using Interfleet.XaltAuthenticationAPI;
using Interfleet.XaltAuthenticationAPI.Services;
using Interfleet.XIUserManagement.Context;
using Interfleet.XIUserManagement.Models;
using Interfleet.XIUserManagement.Repositories;
using Interfleet.XIUserManagement.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Users>();
builder.Services.AddSingleton<Search>();
builder.Services.AddSingleton<LoginViewModel>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<IAuthorization, Authorization>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMemoryCache();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}");



app.Run();

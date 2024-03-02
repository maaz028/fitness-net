using core.Models;
using Fitness_management_system.Services;
using infrastructure.AddionalClasses;
using infrastructure.Repositories;
using infrastructure.Repositories.Trainer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(
    options =>
    {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    });

builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();
builder.Logging.AddNLog();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
});
var connectionString = builder.Configuration.GetConnectionString("FitnessSystemConnection");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString, b => b.MigrationsAssembly("Fitness_management_system")));
builder.Services.AddIdentity<ApplicationUserModel,IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
});
builder.Services.AddScoped<ITrainerRepository, MockTrainerRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<SMTPConfigModel>(builder.Configuration.GetSection("SMTPConfig"));
var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    DeveloperExceptionPageOptions options = new DeveloperExceptionPageOptions()
    {
        SourceCodeLineCount = 10
    };

}
else
{
    app.UseExceptionHandler("/Exception/Error");
    app.UseStatusCodePagesWithReExecute("/Exception/Error/{0}");
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name:"Default",
    pattern:"fitness/{controller=Home}/{action=Index}"
    );


app.Run();

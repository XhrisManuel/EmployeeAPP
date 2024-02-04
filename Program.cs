using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EmployeeAPP.Data;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EmployeeAPPDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeAPPDBContext") ?? throw new InvalidOperationException("Connection string 'EmployeeAPPDBContext' not found.")));

builder.Services.AddDefaultIdentity<EmployeeAPPUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<EmployeeAPPDBContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

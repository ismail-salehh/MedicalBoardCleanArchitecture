using MedicalBoard.Application.Interfaces;
using MedicalBoard.Application.Services;
using MedicalBoard.Domain.Entities;
using MedicalBoard.Infrastructure.Data;
using MedicalBoard.Web.Extensions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddMedicalBoardPersistence(builder.Configuration);
builder.Services.AddMedicalBoardApplicationServices();
builder.Services.AddMedicalBoardAuthentication(builder.Configuration);
builder.Services.AddMedicalBoardAuthorization();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseGlobalExceptionHandling();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRequestLogging();

app.UseAuthentication();
app.UseActiveUserValidation();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
    await DbInitializer.SeedAsync(context, hasher);
}

app.Run();

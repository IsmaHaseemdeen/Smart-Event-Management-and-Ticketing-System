using System;
using System.Linq;
using EventTicketingSystem.Data;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.CommandTimeout(180)));


builder.Services
    .AddControllersWithViews()
    .AddSessionStateTempDataProvider(); 

// session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DB-SEED");

    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        logger.LogInformation("ConnectionString = {cs}", connectionString);
        logger.LogInformation("DB (before migrate) = {db}", db.Database.GetDbConnection().Database);

        db.Database.Migrate();

        logger.LogInformation("DB (after migrate) = {db}", db.Database.GetDbConnection().Database);

        var usersBefore = db.Users.AsNoTracking().Count();
        logger.LogInformation("Users before seed = {count}", usersBefore);

        DbSeeder.Seed(db);

        var usersAfter = db.Users.AsNoTracking().Count();
        logger.LogInformation("Users after seed = {count}", usersAfter);

        logger.LogInformation("Seed done.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Seeding failed!");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quiz.App.Infrastructure;
using Quiz.App.Infrastructure.Repositories;
using Quiz.App.Models;
using Quiz.App.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<QuizDbContext>(options =>
{
    var host = builder.Configuration["DBHOST"] ?? "localhost";
    var port = builder.Configuration["DBPORT"] ?? "3306";
    var password = builder.Configuration["DBPASSWORD"] ?? "201800459";

    var connection = $"server={host};port={port};userid=root;password={password};database=quiz";

    options.UseMySql(connection, ServerVersion.AutoDetect(connection), optionsBuilder =>
    {
        optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery); //https://docs.microsoft.com/pt-br/ef/core/querying/single-split-queries
    });
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<QuizDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<PasswordHasherOptions>(options =>
{
    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ICacheRepository<>), typeof(CacheRepository<>));
builder.Services.AddMemoryCache();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<QuizDbContext>();

    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

    if (pendingMigrations.Any())
    {
        await context.Database.MigrateAsync();
        
        context.Roles.AddRange(new List<IdentityRole>(2)
        {
            new("common"),
            new("admin")
        });

        await context.SaveChangesAsync();

        //todo: create identityService?
        var userManager = services.GetRequiredService<UserManager<User>>();

        var admin = new User("admin", "master", "admin");

        await userManager.CreateAsync(admin, "Teste@123");

        await userManager.AddToRoleAsync(admin, Roles.Admin);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
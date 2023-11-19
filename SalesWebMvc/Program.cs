using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;
using SalesWebMvc.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SalesWebMvcContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SalesWebMvcContext");

    /*options.UseMySql(ServerVersion.AutoDetect(connectionString), builder =>
        builder.MigrationsAssembly("SalesWebMvc"));*/
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), builder =>
        builder.MigrationsAssembly("SalesWebMvc"));

    /*    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));*/
});

builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();

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
else
{
    SeedData(app);

    //Seed Data
    void SeedData(IHost app)
    {
        var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

        using (var scope = scopedFactory?.CreateScope())
        {
            var service = scope?.ServiceProvider.GetService<SeedingService>();
            service?.Seed();
        }
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

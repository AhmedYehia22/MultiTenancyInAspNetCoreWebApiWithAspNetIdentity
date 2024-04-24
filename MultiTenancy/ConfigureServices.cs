using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MultiTenancy;

public static class ConfigureServices
{
    public static IServiceCollection AddTenancy(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<ITenantService, TenantService>();
        var defaultConnectionString = "server=DESKTOP-PMMVBDI\\SQLEXPRESS;Database=SharedDb;Trusted_Connection=True;Encrypt=False";

        var defaultDbProvider = "mssql";

        if (defaultDbProvider.ToLower() == "mssql")
        {
            services.AddDbContext<ApplicationDbContext>(m => m.UseSqlServer());
        }
       using var scope1 = services.BuildServiceProvider().CreateScope();
        var _context = scope1.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _context.Database.SetConnectionString(defaultConnectionString);
        var TenantsIndb = _context.Tenants.ToList();
        foreach (var tenant in TenantsIndb)
        {
            var connectionString = tenant.ConnectionString ?? defaultConnectionString;
            if (tenant.ConnectionString !=null)
            {
                connectionString = $"server=DESKTOP-PMMVBDI\\SQLEXPRESS;Database={tenant.Name};Trusted_Connection=True;Encrypt=False"; 
            }
            using var scope2 = services.BuildServiceProvider().CreateScope();
            var dbContext = scope2.ServiceProvider.GetRequiredService<ApplicationDbContext>();
          
          
            _context.Database.SetConnectionString(connectionString);


            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }
      }
        

        return services;
    }
}
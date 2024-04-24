using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MultiTenancy.Services;

public class TenantService : ITenantService
{
    private HttpContext? _httpContext;
    private AppTenant? _currentTenant;
    private readonly ApplicationDbContext _context;
    private const string  defaultConnection = "Data Source=DESKTOP-PMMVBDI\\SQLEXPRESS;Initial Catalog=SharedDb;Trusted_Connection=True;Encrypt=False;";
    public TenantService(IHttpContextAccessor contextAccessor)
     {
         _httpContext = contextAccessor.HttpContext;
         if(_httpContext is not null)
         {
            /* if(_httpContext.Request.Headers.TryGetValue("tenant", out var tenantId))
             {
                 SetCurrentTenant(tenantId!);
             }
             else
             {
                 SetCurrentTenant("shared");
             }*/
            var claim = _httpContext.User.Claims.FirstOrDefault(e => e.Type == ClaimConstants.TenantId);
            if (claim!= null)
            {
                SetCurrentTenant(claim.Value);
            }
            else
            {
                SetCurrentTenant("Shared");
            }
        }
     }
 
    public string? GetConnectionString()
    {
        var currentConnectionString = _currentTenant is null 
            ? defaultConnection
            : _currentTenant.ConnectionString;

        return currentConnectionString;
    }

    public AppTenant? GetCurrentTenant()
    {
        return _currentTenant;
    }

    public string? GetDatabaseProvider()
    {
        return "mssql";
    }

    private void SetCurrentTenant(string tenantId)
    {
        if (!string.IsNullOrEmpty(tenantId)) {
            _currentTenant = new AppTenant
            {
                Name = tenantId,
                ConnectionString = $"server=DESKTOP-PMMVBDI\\SQLEXPRESS;database={tenantId}Db;trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true;"
            };
        }
       
        if (_currentTenant is null)
        {
            _currentTenant = new AppTenant
            {
                Name = "Shared",
                ConnectionString = defaultConnection
            };
        }
    }
}

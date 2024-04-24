namespace MultiTenancy.Services;

public interface ITenantService
{
    string? GetDatabaseProvider();
    string? GetConnectionString();
    AppTenant? GetCurrentTenant();
}
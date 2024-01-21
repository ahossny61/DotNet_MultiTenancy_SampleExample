namespace MultiTenancy.Services
{
    public interface ITenantService
    {
         string? GetDatabasProvider();
         string? GetConnectionString();
         Tenant? GetCurrentTenant();
    }
}

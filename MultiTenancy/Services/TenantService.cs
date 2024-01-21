
using Microsoft.Extensions.Options;

namespace MultiTenancy.Services
{
    public class TenantService : ITenantService
    {
        private readonly TenantSettings _tenantSettings;
        private Tenant? _currentTenant;
        private HttpContext? _httpContext;

        public TenantService(IHttpContextAccessor contextAccessor, IOptions<TenantSettings> tenantSettings)
        {
            _httpContext = contextAccessor.HttpContext;
            _tenantSettings = tenantSettings.Value;
            if(_httpContext is not null)
            {
                if(_httpContext.Request.Headers.TryGetValue("tenant",out var tenantId))
                {
                    _currentTenant = _tenantSettings.Tenants.FirstOrDefault(t => t.TId == tenantId);
                    if(_currentTenant is  null)
                    {
                        throw new Exception("Invalid Tenant Id");
                    }

                    if (string.IsNullOrEmpty(_currentTenant.ConnectionString))
                    {
                        _currentTenant.ConnectionString = _tenantSettings.Defaults.ConnectionString;
                    }

                }
                else
                {
                    throw new Exception("No Tenant Id ");
                }
            }
        }
        public string? GetConnectionString()
        {
            return _currentTenant?.ConnectionString;
        }

        public Tenant? GetCurrentTenant()
        {
            return _currentTenant;
        }

        public string? GetDatabasProvider()
        {
            return _tenantSettings.Defaults.DBProvider;
        }
    }
}

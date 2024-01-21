namespace MultiTenancy.Settings
{
    public class Tenant
    {
        public string TId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? ConnectionString { get; set; } 

    }
}

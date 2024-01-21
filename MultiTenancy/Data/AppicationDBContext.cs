




namespace MultiTenancy.Data
{
    public class AppicationDBContext :DbContext
    {
        public string TenantId { get; set; }
        private readonly ITenantService _tenantService;
        public AppicationDBContext(DbContextOptions options, ITenantService tenantService) : base(options)
        {
            _tenantService = tenantService;
            TenantId = _tenantService.GetCurrentTenant()?.TId;
        }

        public DbSet<Product>products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasQueryFilter(e => e.TenantId == TenantId);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var _tenantConnection = _tenantService.GetConnectionString();

            if (!string.IsNullOrEmpty(_tenantConnection))
            {
                var dbProvider = _tenantService.GetDatabasProvider();
                if(dbProvider?.ToLower() == "mssqle")
                {
                    optionsBuilder.UseSqlServer(_tenantConnection);
                }

            }
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach(var entity in ChangeTracker.Entries<IMustHaveTenant>().Where(e => e.State == EntityState.Added))
            {
                entity.Entity.TenantId = TenantId;
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

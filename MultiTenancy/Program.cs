

using Microsoft.Extensions.DependencyInjection;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ITenantService, TenantService>();
// Add services to the container.
builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection(nameof(TenantSettings)));

TenantSettings options = new();
builder.Configuration.GetSection(nameof(TenantSettings)).Bind(options);

var defaultDBProvider = options.Defaults.DBProvider;

if(defaultDBProvider.ToLower() == "mssqle")
{
    builder.Services.AddDbContext<AppicationDBContext>(m => m.UseSqlServer());
}

foreach(var tenant in options.Tenants)
{

    var connectString = tenant.ConnectionString?? options.Defaults.ConnectionString;

    using var scope = builder.Services.BuildServiceProvider().CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppicationDBContext>();

    dbContext.Database.SetConnectionString(connectString);
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

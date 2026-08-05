using EnterpriseSaaS.Catalog.Application;
using EnterpriseSaaS.Catalog.Domain;
using EnterpriseSaaS.Catalog.Infrastructure;
using EnterpriseSaaS.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration).WriteTo.Console());
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<CatalogDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Catalog")));
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CatalogDbContext>());
builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<CreateTenantCommand>());
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? []).AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddRateLimiter(o => o.AddFixedWindowLimiter("api", x => { x.PermitLimit = 100; x.Window = TimeSpan.FromMinutes(1); x.QueueLimit = 0; }));
var app = builder.Build();
app.UseExceptionHandler(); app.UseSerilogRequestLogging(); app.UseHttpsRedirection(); app.UseCors(); app.UseRateLimiter();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
if (app.Environment.IsDevelopment())
{
    var tenants = app.MapGroup("/api/tenants").RequireRateLimiting("api");
    tenants.MapGet("/", (ISender sender, CancellationToken ct) => sender.Send(new GetTenantsQuery(), ct));
    tenants.MapPost("/", async (CreateTenantCommand command, ISender sender, CancellationToken ct) => Results.Created("/api/tenants", await sender.Send(command, ct)));
}
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "catalog" }));
app.Run();
public partial class Program;

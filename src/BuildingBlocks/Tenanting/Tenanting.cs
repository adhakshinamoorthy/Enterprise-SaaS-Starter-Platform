using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseSaaS.Tenanting;

public sealed record TenantContext(Guid TenantId, string Identifier, string? ConnectionString = null);
public interface ITenantAccessor { TenantContext? Tenant { get; set; } }
public sealed class TenantAccessor : ITenantAccessor { public TenantContext? Tenant { get; set; } }
public interface ICurrentTenant { Guid Id { get; } string Identifier { get; } string ConnectionString { get; } }
public sealed class CurrentTenant(ITenantAccessor accessor) : ICurrentTenant
{
    private TenantContext Value => accessor.Tenant ?? throw new InvalidOperationException("No tenant is active for this request.");
    public Guid Id => Value.TenantId;
    public string Identifier => Value.Identifier;
    public string ConnectionString => Value.ConnectionString ?? throw new InvalidOperationException("Tenant database is not provisioned.");
}
public interface ITenantStore { Task<TenantContext?> FindAsync(string identifier, CancellationToken cancellationToken); }
public interface ITenantResolver { Task<TenantContext?> ResolveAsync(HttpContext context, CancellationToken cancellationToken); }
public sealed class TenantResolver(ITenantStore store) : ITenantResolver
{
    public async Task<TenantContext?> ResolveAsync(HttpContext context, CancellationToken cancellationToken)
    {
        var identifier = context.Request.Headers["X-Tenant-ID"].FirstOrDefault()
            ?? context.User.FindFirstValue("tenant_id")
            ?? ResolveSubdomain(context.Request.Host.Host);
        return string.IsNullOrWhiteSpace(identifier) ? null : await store.FindAsync(identifier, cancellationToken);
    }
    private static string? ResolveSubdomain(string host)
    { var parts = host.Split('.'); return parts.Length >= 3 ? parts[0] : null; }
}
public sealed class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantResolver resolver, ITenantAccessor accessor)
    {
        var tenant = await resolver.ResolveAsync(context, context.RequestAborted);
        if (tenant is null) { await Results.Problem(statusCode: 400, title: "Tenant required", detail: "Provide a valid X-Tenant-ID header, tenant_id claim, or tenant subdomain.").ExecuteAsync(context); return; }
        accessor.Tenant = tenant;
        try { await next(context); } finally { accessor.Tenant = null; }
    }
}
public static class TenantingExtensions
{
    public static IServiceCollection AddTenanting(this IServiceCollection services) => services
        .AddScoped<ITenantAccessor, TenantAccessor>().AddScoped<ICurrentTenant, CurrentTenant>().AddScoped<ITenantResolver, TenantResolver>();
}

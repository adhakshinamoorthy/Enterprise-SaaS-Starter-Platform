using System.Security.Cryptography;
namespace EnterpriseSaaS.FeatureFlags;

public sealed record FeatureEvaluationContext(Guid TenantId, Guid? UserId, DateTimeOffset Now);
public sealed record FeatureDefinition(string Key, bool Enabled, int Percentage = 100, IReadOnlySet<Guid>? Tenants = null, IReadOnlySet<Guid>? Users = null, DateTimeOffset? StartsAt = null, DateTimeOffset? EndsAt = null);
public interface IFeatureStore { Task<FeatureDefinition?> GetAsync(string key, CancellationToken cancellationToken); }
public interface IFeatureManager { Task<bool> IsEnabledAsync(string key, FeatureEvaluationContext context, CancellationToken cancellationToken = default); }
public sealed class FeatureManager(IFeatureStore store) : IFeatureManager
{
    public async Task<bool> IsEnabledAsync(string key, FeatureEvaluationContext context, CancellationToken cancellationToken = default)
    {
        var f = await store.GetAsync(key, cancellationToken); if (f is null || !f.Enabled) return false;
        if (f.StartsAt > context.Now || f.EndsAt <= context.Now) return false;
        if (f.Users?.Contains(context.UserId ?? Guid.Empty) is true || f.Tenants?.Contains(context.TenantId) is true) return true;
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes($"{key}:{context.TenantId}:{context.UserId}")); return BitConverter.ToUInt32(bytes, 0) % 100 < f.Percentage;
    }
}

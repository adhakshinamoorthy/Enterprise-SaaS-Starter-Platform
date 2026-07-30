namespace EnterpriseSaaS.Audit;

public enum AuditAction { Create, Update, Delete, Login, Logout, PermissionChange, FeatureChange }
public sealed record AuditEntry(Guid Id, Guid? TenantId, Guid? UserId, AuditAction Action, string EntityType, string? EntityId, string? OldValue, string? NewValue, string? IpAddress, string CorrelationId, DateTimeOffset Timestamp);
public interface IAuditWriter { Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default); }

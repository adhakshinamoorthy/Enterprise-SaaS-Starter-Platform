using EnterpriseSaaS.SharedKernel;
namespace EnterpriseSaaS.Catalog.Domain;

public enum TenantStatus { Provisioning, Active, Suspended, Failed }
public sealed class Tenant : AggregateRoot
{
    private Tenant() { }
    public Tenant(string identifier, string name, string connectionString) { Identifier = identifier.Trim().ToLowerInvariant(); Name = name.Trim(); ConnectionString = connectionString; Status = TenantStatus.Provisioning; }
    public string Identifier { get; private set; } = null!; public string Name { get; private set; } = null!; public string ConnectionString { get; private set; } = null!; public TenantStatus Status { get; private set; }
    public void Activate() => Status = TenantStatus.Active; public void Suspend() => Status = TenantStatus.Suspended;
}
public sealed class SubscriptionPlan : AggregateRoot { private SubscriptionPlan() { } public SubscriptionPlan(string name, decimal monthlyPrice) { Name = name; MonthlyPrice = monthlyPrice; } public string Name { get; private set; } = null!; public decimal MonthlyPrice { get; private set; } }
public sealed class FeatureRecord : AggregateRoot { private FeatureRecord() { } public required string Key { get; init; } public bool Enabled { get; set; } public int Percentage { get; set; } = 100; public Guid? TenantId { get; set; } public Guid? UserId { get; set; } public DateTimeOffset? StartsAt { get; set; } public DateTimeOffset? EndsAt { get; set; } }
public sealed class LocalizationResource : Entity { public Guid? TenantId { get; init; } public required string Culture { get; init; } public required string Key { get; init; } public required string Value { get; set; } }
public sealed class FormDefinition : AggregateRoot { public required string Key { get; init; } public int Version { get; init; } = 1; public required string SchemaJson { get; set; } }
public interface ITenantRepository : IRepository<Tenant> { Task<Tenant?> FindByIdentifierAsync(string identifier, CancellationToken cancellationToken = default); Task<IReadOnlyList<Tenant>> ListAsync(CancellationToken cancellationToken = default); }

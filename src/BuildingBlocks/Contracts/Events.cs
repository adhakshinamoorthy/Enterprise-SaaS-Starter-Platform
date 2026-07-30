namespace EnterpriseSaaS.Contracts;

public sealed record TenantProvisioned(Guid TenantId, string Identifier, DateTimeOffset OccurredAt);
public sealed record OrderCreated(Guid TenantId, Guid OrderId, decimal Total, DateTimeOffset OccurredAt);

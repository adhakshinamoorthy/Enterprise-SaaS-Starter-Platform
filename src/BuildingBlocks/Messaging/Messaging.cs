namespace EnterpriseSaaS.Messaging;

public interface IIntegrationEventPublisher { Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class; }
public sealed class OutboxMessage { public Guid Id { get; init; } = Guid.NewGuid(); public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow; public required string Type { get; init; } public required string Payload { get; init; } public DateTimeOffset? ProcessedAt { get; set; } public string? Error { get; set; } }
public sealed class InboxMessage { public required Guid Id { get; init; } public DateTimeOffset ProcessedAt { get; init; } = DateTimeOffset.UtcNow; public required string Consumer { get; init; } }

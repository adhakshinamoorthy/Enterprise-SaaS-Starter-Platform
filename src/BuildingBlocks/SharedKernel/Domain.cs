namespace EnterpriseSaaS.SharedKernel;

public interface IDomainEvent { DateTimeOffset OccurredAt { get; } }

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;
    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

public abstract class AggregateRoot : Entity;

public interface IRepository<T> where T : AggregateRoot
{
    Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Remove(T entity);
}

public interface IUnitOfWork { Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); }

public abstract class BusinessException(string message, string code) : Exception(message)
{ public string Code { get; } = code; }

public sealed class NotFoundException(string resource, object key)
    : BusinessException($"{resource} '{key}' was not found.", "not_found");

using EnterpriseSaaS.Catalog.Domain;
using EnterpriseSaaS.SharedKernel;
using MediatR;
namespace EnterpriseSaaS.Catalog.Application;

public sealed record CreateTenantCommand(string Identifier, string Name, string ConnectionString) : IRequest<TenantDto>;
public sealed record GetTenantsQuery : IRequest<IReadOnlyList<TenantDto>>;
public sealed record TenantDto(Guid Id, string Identifier, string Name, string Status);
public sealed class CreateTenantHandler(ITenantRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<CreateTenantCommand, TenantDto>
{ public async Task<TenantDto> Handle(CreateTenantCommand r, CancellationToken ct) { if (await repository.FindByIdentifierAsync(r.Identifier, ct) is not null) throw new InvalidOperationException("Tenant identifier already exists."); var tenant = new Tenant(r.Identifier, r.Name, r.ConnectionString); await repository.AddAsync(tenant, ct); tenant.Activate(); await unitOfWork.SaveChangesAsync(ct); return new(tenant.Id, tenant.Identifier, tenant.Name, tenant.Status.ToString()); } }
public sealed class GetTenantsHandler(ITenantRepository repository) : IRequestHandler<GetTenantsQuery, IReadOnlyList<TenantDto>>
{ public async Task<IReadOnlyList<TenantDto>> Handle(GetTenantsQuery r, CancellationToken ct) => (await repository.ListAsync(ct)).Select(x => new TenantDto(x.Id, x.Identifier, x.Name, x.Status.ToString())).ToArray(); }

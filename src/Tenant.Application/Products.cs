using EnterpriseSaaS.SharedKernel;
using EnterpriseSaaS.Tenant.Domain;
using EnterpriseSaaS.Tenanting;
using MediatR;
namespace EnterpriseSaaS.Tenant.Application;

public sealed record CreateProductCommand(string Sku, string Name, decimal Price) : IRequest<ProductDto>;
public sealed record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;
public sealed record ProductDto(Guid Id, string Sku, string Name, decimal Price);
public sealed class CreateProductHandler(ICurrentTenant tenant, IProductRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, ProductDto>
{ public async Task<ProductDto> Handle(CreateProductCommand r, CancellationToken ct) { var product = new Product(tenant.Id, r.Sku, r.Name, r.Price); await repository.AddAsync(product, ct); await unitOfWork.SaveChangesAsync(ct); return new(product.Id, product.Sku, product.Name, product.Price); } }
public sealed class GetProductsHandler(IProductRepository repository) : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{ public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery r, CancellationToken ct) => (await repository.ListAsync(ct)).Select(x => new ProductDto(x.Id, x.Sku, x.Name, x.Price)).ToArray(); }

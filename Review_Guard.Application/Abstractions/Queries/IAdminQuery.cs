using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Abstractions.Queries;

public interface IAdminQuery : IGenericReadRepository<Admin>
{
    IQueryable<Admin> GetAll(CancellationToken cancellationToken = default);

    Task<Admin?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Admin?> ByNameAsync(string name, CancellationToken cancellationToken = default);
}

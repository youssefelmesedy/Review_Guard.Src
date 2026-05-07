using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Abstractions.Repositories.BranchRepository;

public interface IReadBranchRepository : IGenericReadRepository<Branch>
{
}

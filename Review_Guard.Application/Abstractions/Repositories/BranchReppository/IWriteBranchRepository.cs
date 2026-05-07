using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Abstractions.Repositories.BranchReppository;

public interface IWriteBranchRepository : IGenericWriteRepository<Branch>
{
}

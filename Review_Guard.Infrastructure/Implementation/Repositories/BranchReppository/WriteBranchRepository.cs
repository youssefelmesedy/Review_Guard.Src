using Review_Guard.Application.Abstractions.Repositories.BranchReppository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.BranchReppository;

internal sealed class WriteBranchRepository : GenericWriteRepository<Branch>, IWriteBranchRepository
{
    public WriteBranchRepository(AppDbContext context) : base(context)
    {
    }
}

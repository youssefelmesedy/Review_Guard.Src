using Review_Guard.Application.Abstractions.Repositories.BranchRepository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.BranchRepository;

internal sealed class ReadBranchRepository : GenericReadRepository<Branch>, IReadBranchRepository
{
    public ReadBranchRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}

using Review_Guard.Application.Abstractions.Repositories.ProofRepostory;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.ProofRepostory;

internal sealed class ReadProofRepository : GenericReadRepository<Proof>, IReadProofRepository
{
    public ReadProofRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}

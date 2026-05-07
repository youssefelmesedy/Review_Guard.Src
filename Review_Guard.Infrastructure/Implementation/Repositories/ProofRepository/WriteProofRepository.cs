using Review_Guard.Application.Abstractions.Repositories.ProofRepostory;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.ProofRepository;

internal sealed class WriteProofRepository : GenericWriteRepository<Proof>, IWriteProofRepository
{
    public WriteProofRepository(AppDbContext context) : base(context)
    {
    }
}

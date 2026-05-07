using Review_Guard.Application.Abstractions.Repositories.BusinessReppository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.BusinessReppository;

internal sealed class WriteBusinessRepository : GenericWriteRepository<Business>, IWriteBusinessRepository
{
    public WriteBusinessRepository(AppDbContext context) : base(context)
    {
    }
}

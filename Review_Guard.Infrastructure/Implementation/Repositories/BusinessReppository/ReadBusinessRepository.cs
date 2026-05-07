using Review_Guard.Application.Abstractions.Repositories.BusinessReppository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.BusinessReppository;

internal sealed class ReadBusinessRepository : GenericReadRepository<Business>, IReadBusinessRepository
{
    public ReadBusinessRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}


using Review_Guard.Application.Abstractions.Repositories.AdminRepository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.AdminRepository;

internal sealed class ReadAdminRepository : GenericReadRepository<Admin>, IReadAdminRepository
{
    public ReadAdminRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}


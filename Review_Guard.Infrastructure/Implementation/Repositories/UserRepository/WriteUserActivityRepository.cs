using Review_Guard.Application.Abstractions.Repositories.UserRepository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.UserRepository;

internal sealed class WriteUserActivityRepository : GenericWriteRepository<UserActivity>, IWriteUserActivityRepository
{
    public WriteUserActivityRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}

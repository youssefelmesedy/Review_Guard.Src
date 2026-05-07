using Review_Guard.Application.Abstractions.Repositories.UserRepository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.UserRepository;

internal sealed class ReadUserActivityRepository : GenericReadRepository<UserActivity>, IReadUserActivityRepository
{
    public ReadUserActivityRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}

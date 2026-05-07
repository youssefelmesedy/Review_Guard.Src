using Review_Guard.Application.Abstractions.Repositories.UserRepository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.UserRepository;

internal sealed class ReadUserRepository : GenericReadRepository<User>, IReadUserRepository
{
    public ReadUserRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<User?> GetByIdWithRewardsAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await _appDbContext.Users
            .Include(u => u.Rewards)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}

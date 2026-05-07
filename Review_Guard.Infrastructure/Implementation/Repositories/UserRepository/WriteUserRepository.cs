using Review_Guard.Application.Abstractions.Repositories.UserRepository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.UserRepository;

internal sealed class WriteUserRepository : GenericWriteRepository<User>, IWriteUserRepository
{
    public WriteUserRepository(AppDbContext context) : base(context)
    {
    }
}

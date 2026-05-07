
using Review_Guard.Application.Abstractions.Repositories.ReportReppository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.ReportReppository;

internal sealed class ReadReportRepository : GenericReadRepository<Report>, IReadReportRepository
{
    public ReadReportRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}
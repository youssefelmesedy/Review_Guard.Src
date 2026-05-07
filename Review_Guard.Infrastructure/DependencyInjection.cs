namespace Review_Guard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // ── Database ───────────────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql
                    .EnableRetryOnFailure(maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null)
                    .CommandTimeout(30)));

        //// ── Unit of Work ───────────────────────────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ---- caching services ----------------------------------------------------
        // Memory Cache
        services.AddMemoryCache();

        // Distributed Cache (Redis أو In-Memory مؤقت)
        services.AddDistributedMemoryCache(); // 👈 مؤقت (بدل Redis)

        // Hybrid Cache Service
        services.AddSingleton<ICacheService, MemoryCacheService>();

        //// ── Core Repositories ──────────────────────────────────────────────────
        services.AddScoped(typeof(IGenericReadRepository<>), typeof(GenericReadRepository<>));
        services.AddScoped(typeof(IGenericWriteRepository<>), typeof(GenericWriteRepository<>));

        services.AddScoped<IRewardService, RewardService>();

        services.AddScoped<IReadAdminRepository, ReadAdminRepository>();
        //services.AddScoped<IWriteAdminRepository, WriteAdminRepository>();

        services.AddScoped<IReadBranchRepository, ReadBranchRepository>();
        services.AddScoped<IWriteBranchRepository, WriteBranchRepository>();

        services.AddScoped<IReadBusinessRepository, ReadBusinessRepository>();
        services.AddScoped<IWriteBusinessRepository, WriteBusinessRepository>();

        services.AddScoped<IReadProofRepository, ReadProofRepository>();
        services.AddScoped<IWriteProofRepository, WriteProofRepository>();

        services.AddScoped<IReadReportRepository, ReadReportRepository>();
        services.AddScoped<IWriteReportRepository, WriteReportRepository>();

        services.AddScoped<IReadReviewRepository, ReadReviewRepository>();
        services.AddScoped<IWriteReviewRepository, WriteReviewRepository>();

        services.AddScoped<IReadUserRepository, ReadUserRepository>();
        services.AddScoped<IWriteUserRepository, WriteUserRepository>();

        services.AddScoped<IReadUserActivityRepository, ReadUserActivityRepository>();
        services.AddScoped<IWriteUserActivityRepository, WriteUserActivityRepository>();

        //// ── Dashboard Repositories (read-model / CQRS read side) ───────────────
        //services.AddScoped<IUserDashboardRepository, UserDashboardRepository>();
        //services.AddScoped<IOwnerDashboardRepository, OwnerDashboardRepository>();
        //services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();

        //// ── Settings ───────────────────────────────────────────────────────────
        //services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        //services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        //services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));

        //// ── Application + Infrastructure Services ─────────────────────────────
        //services.AddScoped<IJwtService, JwtService>();
        //services.AddScoped<IPasswordHasher, PasswordHasher>();
        //services.AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>();
        //services.AddScoped<IEmailService, EmailService>();
        //services.AddScoped<IFileStorageService, LocalFileStorageService>();
        //services.AddScoped<IRiskScoreService, RiskScoreService>();
        //services.AddScoped<IWeightedRatingService, WeightedRatingService>();

        return services;
    }
}

// Extension method for configuring custom logging
public static class LoggingExtensions
{
    public static ILoggingBuilder AddCustomLogging(this ILoggingBuilder builder)
    {
        builder.ClearProviders();

        builder.AddConsole(options =>
        {
            options.FormatterName = "colored";
        });

        builder.Services.AddSingleton<ConsoleFormatter, ColoredConsoleFormatter>();

        return builder;
    }
}
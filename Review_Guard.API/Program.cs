using Review_Guard.API.Extensions;
using Review_Guard.Infrastructure;

namespace Review_Guard.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers
        builder.Services.AddControllers()
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.PropertyNamingPolicy =
                    System.Text.Json.JsonNamingPolicy.CamelCase;

                opts.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter());

                opts.JsonSerializerOptions.DefaultIgnoreCondition =
                    System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

        // Localization
        builder.Services.AddLocalizationServices();

        // Caching
        builder.Services.AddMemoryCache();
        builder.Services.AddResponseCaching();

        // Core
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();

        // Layers
        //builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Logging.AddCustomLogging();

        // Cross-cutting
        //builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddSwaggerWithAuth();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}

using System.Text.Json;
using IOT_WateringSensor.Database;
using Microsoft.AspNetCore.Http.Json;

namespace IOT_WateringSensor;

public static class ServiceCollectionExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        
        services.AddDbContext<WaterSensorDbContext>();
        services.AddLogging();
        services.AddHttpContextAccessor();
        
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.IncludeFields = true;
        });
        
        services.AddEndpointsApiExplorer();

        services.AddAuthentication();
        services.AddAuthorization();
    }
}
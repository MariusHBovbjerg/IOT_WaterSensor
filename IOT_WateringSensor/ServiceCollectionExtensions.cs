using System.Text.Json;
using System.Text.Json.Serialization;
using IOT_WateringSensor.Areas.Identity;
using IOT_WateringSensor.Database;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;

namespace IOT_WateringSensor;

public static class ServiceCollectionExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        
        services.AddDbContext<WaterSensorDbContext>();
        services.AddDefaultIdentity<IdentityUser>(
                options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<WaterSensorDbContext>();
        services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
        
        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.IncludeFields = true;
        });
        
        services.AddEndpointsApiExplorer();

        services.AddAuthentication();
        services.AddAuthorization();
        
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddMudServices();
    }
}
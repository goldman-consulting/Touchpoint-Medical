using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StackExchange.Redis;

using TouchpointMedical.Configuration;
using TouchpointMedical.Integration;

namespace TouchpointMedical.Services
{
    // just for a typed logger category
    public sealed class RedisConnectionFactory { } 

    public static class TouchpointMedicalServiceRegistration
    {
        public static void AddTouchpoint(this IServiceCollection services, ConfigurationManager config)
        {
            services.AddScoped<TouchpointApiTokenService>();
            services.AddScoped<TouchpointApiService>();

            services.AddHttpClient(nameof(TouchpointApiService))
                .AddHttpMessageHandler(serviceProvider =>
                    new AuthRetryHandler(serviceProvider.GetRequiredService<TouchpointApiTokenService>()));


            var configSection = config.GetSection("Touchpoint");
            if (configSection.Exists())
            {
                services.Configure<TouchpointOptions>(configSection);

                var options = configSection.Get<TouchpointOptions>();

                if (options != null)
                {
                    // Make path configurable, still default to App_Data
                    var configuredPath = options.FacilitySettingsDataPath;

                    config.AddJsonFile(configuredPath, optional: true, reloadOnChange: true);
                    services.Configure<FacilitiesSettingsMap>(config);

                    services.AddSingleton<FacilitySettingsFactory>();

                    if (!string.IsNullOrEmpty(options.Connection))
                    {
                        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
                        {
                            var logger = serviceProvider.GetRequiredService<ILogger<RedisConnectionFactory>>();

                            //var multiplexerConfiguration = ConfigurationOptions.Parse(options.Connection);


                            //return ConnectionMultiplexer.Connect(multiplexerConfiguration);

                            for (int attempt = 1; attempt <= 5; attempt++)
                            {
                                try
                                {
                                    var connection = ConnectionMultiplexer.Connect(options.Connection);
                                    if (connection.IsConnected)
                                    {
                                        return connection;
                                    }

                                }
                                catch (RedisConnectionException ex)
                                {
                                    logger.LogWarning("Redis connection failed. {Attempt}", ex.Message);

                                    Thread.Sleep(2000);
                                }
                            }

                            logger.LogCritical("Failed to connect to Redis after multiple attempts.");

                            throw new TouchpointServiceException("Failed to connect to Redis after multiple attempts.");

                        });

                        services.AddStackExchangeRedisCache(co =>
                        {
                            co.Configuration = options.Connection;
                        });
                    }
                }
            }
        }

    }

}
    
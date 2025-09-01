using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Core;

using System;
using System.Security.Cryptography.X509Certificates;

using TouchpointMedical.Auth;
using TouchpointMedical.Integration.PointClickCare.Configuration;
using TouchpointMedical.Integration.PointClickCare.Infrastructure;
using TouchpointMedical.Integration.PointClickCare.Integration;
using TouchpointMedical.Integration.PointClickCare.Services.Background;
using TouchpointMedical.Services;

namespace TouchpointMedical.Integration.PointClickCare.Services
{
    public static class PointClickCareServiceRegistration
    {
        public static void AddPointClickCare(this IServiceCollection services, IConfiguration config)
        {
            var configSection = config.GetSection("PCC");
            if (configSection.Exists())
            {
                services.Configure<PointClickCareOptions>(configSection);

                var options = configSection.Get<PointClickCareOptions>()!;

                services.AddScoped<PointClickCareApiTokenService>();
                services.AddScoped<PointClickCareApiService>();

                //Inbound message queue handler
                services.AddTransient<PointClickCareRedisMessageQueue>();

                //Outbound message queue handler
                services.AddHostedService<PointClickCareRedisDebounceWorker>();

                try
                {
                    services.AddSingleton(serviceProvider =>
                    {
                        // attempt to open the PCC cert here with the same path/password you use in DI
                        var pointClickCarePrivateCert = X509CertificateLoader.LoadPkcs12FromFile(
                            options.ClientCertificateDataPath,
                            options.ClientCertificatePassword,
                            X509KeyStorageFlags.MachineKeySet |
                            X509KeyStorageFlags.PersistKeySet |
                            X509KeyStorageFlags.Exportable,
                            new Pkcs12LoaderLimits { PreserveStorageProvider = true });
                        
                        Log.Information("PCC client certificate loaded OK: {Subject}", pointClickCarePrivateCert.Subject);

                        return pointClickCarePrivateCert;
                    });
                    
                    services.AddHttpClient(nameof(PointClickCareApiTokenService))
                        .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
                        {
                            var privateCert = serviceProvider.GetRequiredService<X509Certificate2>();
                            var handler = new HttpClientHandler();
                            handler.ClientCertificates.Add(privateCert);

                            return handler;
                        });

                    services.AddHttpClient(nameof(PointClickCareApiService))
                        .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
                        {
                            var privateCert = serviceProvider.GetRequiredService<X509Certificate2>();
                            var handler = new HttpClientHandler();
                            handler.ClientCertificates.Add(privateCert);

                            return handler;
                        })
                        .AddHttpMessageHandler(serviceProvider => new AuthRetryHandler(serviceProvider.GetRequiredService<PointClickCareApiTokenService>()));

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to load PCC client certificate from {Path}", options.ClientCertificateDataPath);
                }




            }
            else
            {
                throw new TouchpointServiceException("PointClickCare Service Registration Failed: No PCC Configuration Options");
            }

        }
    }
}

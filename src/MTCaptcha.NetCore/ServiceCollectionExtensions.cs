using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTCaptcha.NetCore.Interfaces;
using MTCaptcha.NetCore.Options;
using MTCaptcha.NetCore.Services;

namespace MTCaptcha.NetCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MTCaptcha services to the DI container using Action{MTCaptchaOptions}.
        /// </summary>
        public static IServiceCollection AddMTCaptcha(
            this IServiceCollection services,
            Action<MTCaptchaOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.Configure(configure);
            services.AddHttpClient<IMTCaptchaService, MTCaptchaService>();

            return services;
        }

        /// <summary>
        /// Adds MTCaptcha services using configuration binding (appsettings.json).
        /// </summary>
        public static IServiceCollection AddMTCaptcha(
            this IServiceCollection services,
            IConfigurationSection section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            services.Configure<MTCaptchaOptions>(section);
            services.AddHttpClient<IMTCaptchaService, MTCaptchaService>();

            return services;
        }
    }
}

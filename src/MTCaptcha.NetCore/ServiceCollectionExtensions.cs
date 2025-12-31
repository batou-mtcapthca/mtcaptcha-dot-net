using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MTCaptcha.NetCore.Interfaces;
using MTCaptcha.NetCore.Options;
using MTCaptcha.NetCore.Services;

namespace MTCaptcha.NetCore.Extensions
{
    /// <summary>
    /// Extension methods for adding MTCaptcha services to the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MTCaptcha services to the DI container using an action to configure options.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configure">An action to configure the <see cref="MTCaptchaOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configure"/> is null.</exception>
        public static IServiceCollection AddMTCaptcha(
            this IServiceCollection services,
            Action<MTCaptchaOptions> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.Configure(configure);
            services.AddSingleton<IValidateOptions<MTCaptchaOptions>, MTCaptchaOptionsValidator>();
            services.AddHttpClient<IMTCaptchaService, MTCaptchaService>();

            return services;
        }

        /// <summary>
        /// Adds MTCaptcha services using configuration binding from appsettings.json or other configuration sources.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="section">The configuration section containing MTCaptcha options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="section"/> is null.</exception>
        public static IServiceCollection AddMTCaptcha(
            this IServiceCollection services,
            IConfigurationSection section)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            services.Configure<MTCaptchaOptions>(section);
            services.AddSingleton<IValidateOptions<MTCaptchaOptions>, MTCaptchaOptionsValidator>();
            services.AddHttpClient<IMTCaptchaService, MTCaptchaService>();

            return services;
        }
    }
}

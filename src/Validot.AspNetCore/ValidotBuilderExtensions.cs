namespace Validot.AspNetCore
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Validot.AspNetCore.Internal;
    using Validot.DependencyInjection;

    /// <summary>
    /// Validot-related <see cref="ValidotBuilder"/> extensions.
    /// </summary>
    public static class ValidotBuilderExtensions
    {
        /// <summary>
        /// Adds Validot integration with ASP.NET Core integration.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ValidotBuilder AddAspNetCoreIntegration(this ValidotBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            IServiceCollection services = builder.Services;

            if (services.Any(x => x.ServiceType == typeof(ValidotAspNetCoreIntegrationMarker)))
            {
                return builder;
            }

            services.AddSingleton<ValidotAspNetCoreIntegrationMarker>();

            services.Add(ServiceDescriptor.Singleton<IObjectModelValidator, ValidotObjectModelValidator>(serviceProvider =>
            {
                MvcOptions options = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
                IModelMetadataProvider metadataProvider = serviceProvider.GetRequiredService<IModelMetadataProvider>();

                return new ValidotObjectModelValidator(metadataProvider, options.ModelValidatorProviders, false);
            }));

            services.AddOptions();
            services.AddSingleton<IPostConfigureOptions<MvcOptions>, ConfigureMvcOptions>();

            return builder;
        }

        /// <summary>
        /// Registers <typeparamref name="T"/> as <see cref="IValidationResultLanguageProvider"/>;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static ValidotBuilder AddValidationResultLanguageProvider<T>(this ValidotBuilder builder,
                                                                            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where T : class, IValidationResultLanguageProvider
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Services.Replace(
                ServiceDescriptor.Describe(typeof(IValidationResultLanguageProvider), typeof(T), serviceLifetime));

            return builder;
        }
    }
}
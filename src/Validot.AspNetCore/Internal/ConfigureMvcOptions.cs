namespace Validot.AspNetCore.Internal
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Configures <see cref="MvcOptions"/>.
    /// </summary>
    internal sealed class ConfigureMvcOptions : IPostConfigureOptions<MvcOptions>
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigureMvcOptions"/>.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ConfigureMvcOptions(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public void PostConfigure(string name, MvcOptions options)
        {
            // Check if the providers have already been added.
            // We shouldn't have to do this, but there's a bug in the ASP.NET Core integration
            // testing components that can cause ConfigureServices to be called multple times
            // meaning we end up with duplicates.

            if (!options.ModelMetadataDetailsProviders.Any(x => x is ValidotBindingMetadataProvider))
            {
                options.ModelMetadataDetailsProviders.Add(new ValidotBindingMetadataProvider());
            }

            if (!options.ModelValidatorProviders.Any(x => x is ValidotModelValidatorProvider))
            {
                ValidotModelValidatorProvider validotModelValidatorProvider = new(_serviceProvider);

                options.ModelValidatorProviders.Insert(0, validotModelValidatorProvider);
            }
        }
    }
}

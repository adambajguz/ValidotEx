namespace Validot.DependencyInjection
{
    using Validot.DependencyInjection.Internal;

    /// <summary>
    /// Validot-related <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Validot to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ValidotBuilder AddValidot(this IServiceCollection services)
        {
            services.TryAddSingleton<IValidotValidatorProvider, ValidotValidatorProvider>();

            return new ValidotBuilder(services);
        }
    }
}
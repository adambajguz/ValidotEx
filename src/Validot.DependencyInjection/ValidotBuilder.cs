namespace Validot.DependencyInjection
{
    /// <summary>
    /// Validot builder.
    /// </summary>
    public sealed class ValidotBuilder
    {
        /// <summary>
        /// Services collection.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ValidotBuilder"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ValidotBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}
namespace Validot.DependencyInjection
{
    /// <summary>
    /// <see cref="IValidotValidatorProvider"/> extensions.
    /// </summary>
    public static class ValidotValidatorProviderExtensions
    {
        /// <summary>
        /// Gets validator for the specified model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IValidotValidator<T>? GetRequiredValidator<T>(this IValidotValidatorProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider);

            IValidotValidator<T>? validator = provider.GetValidator<T>();

            return validator ?? throw new InvalidOperationException($"No validator for type '{typeof(T)}' has been registered.");
        }

        /// <summary>
        /// Gets validator for the specified model.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static IValidotValidator GetRequiredValidator(this IValidotValidatorProvider provider, Type modelType)
        {
            ArgumentNullException.ThrowIfNull(provider);

            IValidotValidator? validator = provider.GetValidator(modelType);

            return validator ?? throw new InvalidOperationException($"No validator for type '{modelType}' has been registered.");
        }
    }
}

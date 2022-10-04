namespace Validot.DependencyInjection
{
    /// <summary>
    /// Validator provider.
    /// </summary>
    public interface IValidotValidatorProvider
    {
        /// <summary>
        /// Gets validator for the specified model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IValidotValidator<T>? GetValidator<T>();

        /// <summary>
        /// Gets validator for the specified model.
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        IValidotValidator? GetValidator(Type modelType);

        /// <summary>
        /// Gets all Validot validators.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IValidotValidator> GetValidators();
    }
}

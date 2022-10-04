namespace Validot.DependencyInjection
{
    using Validot;
    using Validot.Errors;
    using Validot.Results;
    using Validot.Settings;

    /// <summary>
    /// Validot validator proxy.
    /// </summary>
    public interface IValidotValidator
    {
        /// <summary>
        /// Validated model type.
        /// </summary>
        Type ModelType { get; }

        /// <summary>
        /// Error registry.
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyList<IError>> ErrorRegistry { get; }

        /// <summary>
        /// Gets settings of this <see cref="IValidator{T}"/> instance.
        /// </summary>
        IValidatorSettings Settings { get; }

        /// <summary>
        /// Gets the validation result that contains all possible paths and errors described in the specification.
        /// It's the specification in a form of <see cref="IValidationResult"/>.
        /// For collection, the path contains only '#' instead of the item's index.
        /// For reference loop's root, the error is replaced with the single message under the key 'Global.ReferenceLoop'.
        /// </summary>
        IValidationResult Template { get; }

        /// <summary>
        /// Quickly verifies whether the model is valid (according to the specification) or not.
        /// This is highly-optimized version of <see cref="IValidator{T}.Validate"/>, but it doesn't return any information about errors.
        /// </summary>
        /// <param name="model">The model to be validated.</param>
        /// <returns>True, if model is valid and there are no errors according to the specification. Otherwise - false.</returns>
        bool IsValid(object model);

        /// <summary>
        /// Validates the model against the specification. Returns <see cref="IValidationResult"/> object that contains full information about the errors found during the validation process.
        /// WARNING! The returned <see cref="IValidationResult"/> object is internally coupled with the instance of <see cref="IValidator{T}"/> that created it.
        /// It's safe to use its members to get the information you want and process them further, but don't cache the instance of <see cref="IValidationResult"/> itself or pass it around your system too much.
        /// </summary>
        /// <param name="model">The model to be validated.</param>
        /// <param name="failFast">If true, the validation process will stop after detecting the first error. Otherwise, full validation is performed.</param>
        /// <returns>Full information (in a form of <see cref="IValidationResult"/> about the errors found during the validation process, their location, messages and codes.</returns>
        IValidationResult Validate(object model, bool failFast = false);
    }

    /// <summary>
    /// Validot <see cref="IValidator{T}"/> proxy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValidotValidator<T> : IValidotValidator
    {
        Type IValidotValidator.ModelType => typeof(T);

        bool IValidotValidator.IsValid(object model)
        {
            return IsValid((T)model);
        }

        IValidationResult IValidotValidator.Validate(object model, bool failFast)
        {
            return Validate((T)model, failFast);
        }

        /// <summary>
        /// Quickly verifies whether the model is valid (according to the specification) or not.
        /// This is highly-optimized version of <see cref="IValidator{T}.Validate"/>, but it doesn't return any information about errors.
        /// </summary>
        /// <param name="model">The model to be validated.</param>
        /// <returns>True, if model is valid and there are no errors according to the specification. Otherwise - false.</returns>
        bool IsValid(T model);

        /// <summary>
        /// Validates the model against the specification. Returns <see cref="IValidationResult"/> object that contains full information about the errors found during the validation process.
        /// WARNING! The returned <see cref="IValidationResult"/> object is internally coupled with the instance of <see cref="IValidator{T}"/> that created it.
        /// It's safe to use its members to get the information you want and process them further, but don't cache the instance of <see cref="IValidationResult"/> itself or pass it around your system too much.
        /// </summary>
        /// <param name="model">The model to be validated.</param>
        /// <param name="failFast">If true, the validation process will stop after detecting the first error. Otherwise, full validation is performed.</param>
        /// <returns>Full information (in a form of <see cref="IValidationResult"/> about the errors found during the validation process, their location, messages and codes.</returns>
        IValidationResult Validate(T model, bool failFast = false);
    }
}
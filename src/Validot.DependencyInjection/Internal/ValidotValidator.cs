namespace Validot.DependencyInjection.Internal
{
    using System.Collections.Generic;
    using Validot;
    using Validot.Errors;
    using Validot.Results;
    using Validot.Settings;

    /// <summary>
    /// Default implementation of <see cref="IValidotValidator{T}"/>.
    /// </summary>
    internal sealed class ValidotValidator<T> : IValidotValidator<T>
    {
        private readonly IValidator<T> _validator;

        /// <inheritdoc/>
        public IValidatorSettings Settings => _validator.Settings;

        /// <inheritdoc/>
        public IValidationResult Template => _validator.Template;

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, IReadOnlyList<IError>> ErrorRegistry { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="IValidotValidator{T}"/>.
        /// </summary>
        /// <param name="validator"></param>
        public ValidotValidator(IValidator<T> validator)
        {
            _validator = validator;
            ErrorRegistry = ValidotErrorRegistryProvider.Get(validator);
        }

        /// <inheritdoc/>
        public bool IsValid(T model)
        {
            return _validator.IsValid(model);
        }

        /// <inheritdoc/>
        public IValidationResult Validate(T model, bool failFast = false)
        {
            return _validator.Validate(model, failFast);
        }
    }
}

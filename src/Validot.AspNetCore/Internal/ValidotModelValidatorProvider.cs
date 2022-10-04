namespace Validot.AspNetCore.Internal
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    /// <summary>
    /// <see cref="IModelValidatorProvider"/> implementation only used for child properties.
    /// </summary>
    internal class ValidotModelValidatorProvider : IModelValidatorProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ObjectFactory _validotModelValidatorFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidotModelValidatorProvider"/>.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ValidotModelValidatorProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _validotModelValidatorFactory = ActivatorUtilities.CreateFactory(typeof(ValidotModelValidator), new[] { typeof(ModelMetadata) });
        }

        /// <inheritdoc/>
        public virtual void CreateValidators(ModelValidatorProviderContext context)
        {
            ValidotModelValidator validotModelValidator = CreateValidator(context.ModelMetadata);

            context.Results.Add(new ValidatorItem
            {
                IsReusable = true,
                Validator = validotModelValidator,
            });
        }

        private ValidotModelValidator CreateValidator(ModelMetadata modelMetadata)
        {
            return (ValidotModelValidator)_validotModelValidatorFactory(_serviceProvider, new object[] { modelMetadata });
        }
    }
}
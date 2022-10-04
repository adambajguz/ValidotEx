namespace Validot.AspNetCore
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Validot.AspNetCore.Internal;

    internal class ValidotObjectModelValidator : ObjectModelValidator
    {
        private readonly bool _runMvcValidation;
        private readonly ValidotModelValidatorProvider _validotProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidotObjectModelValidator"/>.
        /// </summary>
        /// <param name="modelMetadataProvider"></param>
        /// <param name="validatorProviders"></param>
        /// <param name="runMvcValidation"></param>
        public ValidotObjectModelValidator(IModelMetadataProvider modelMetadataProvider,
                                           IList<IModelValidatorProvider> validatorProviders,
                                           bool runMvcValidation)
            : base(modelMetadataProvider, validatorProviders)
        {
            _runMvcValidation = runMvcValidation;
            _validotProvider = (ValidotModelValidatorProvider)validatorProviders.Single(x => x is ValidotModelValidatorProvider);
        }

        /// <inheritdoc/>
        public override ValidationVisitor GetValidationVisitor(ActionContext actionContext,
                                                               IModelValidatorProvider validatorProvider,
                                                               ValidatorCache validatorCache,
                                                               IModelMetadataProvider metadataProvider,
                                                               ValidationStateDictionary? validationState)
        {
            // Setting as to whether we should run only Validot or Validot + the other validator providers
            IModelValidatorProvider validatorProviderToUse = _runMvcValidation
                ? validatorProvider
                : _validotProvider;

            ValidotValidationVisitor visitor = new(
                actionContext,
                validatorProviderToUse,
                validatorCache,
                metadataProvider,
                validationState);

            return visitor;
        }
    }
}
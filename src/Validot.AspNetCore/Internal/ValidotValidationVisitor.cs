namespace Validot.AspNetCore.Internal
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    /// <summary>
    /// Validot <see cref="ValidationVisitor"/>.
    /// </summary>
    /// <remarks>Based on: https://github.com/FluentValidation/FluentValidation.AspNetCore</remarks>
    internal sealed class ValidotValidationVisitor : ValidationVisitor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ValidotValidationVisitor"/>.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="validatorProvider"></param>
        /// <param name="validatorCache"></param>
        /// <param name="metadataProvider"></param>
        /// <param name="validationState"></param>
        public ValidotValidationVisitor(ActionContext actionContext,
                                        IModelValidatorProvider validatorProvider,
                                        ValidatorCache validatorCache,
                                        IModelMetadataProvider metadataProvider,
                                        ValidationStateDictionary? validationState)
            : base(actionContext, validatorProvider, validatorCache, metadataProvider, validationState)
        {
            ValidateComplexTypesIfChildValidationFails = true;
        }

        /// <inheritdoc/>
        public override bool Validate(ModelMetadata? metadata, string? key, object? model, bool alwaysValidateAtTopLevel)
        {
            // This overload needs to be in place for both .NET 5 and .NET Core 2/3

            bool BaseValidate()
            {
                return base.Validate(metadata, key, model, alwaysValidateAtTopLevel);
            }

            return ValidateInternal(metadata, key, model, BaseValidate);
        }

        /// <inheritdoc/>
        public override bool Validate(ModelMetadata? metadata, string? key, object? model, bool alwaysValidateAtTopLevel, object? container)
        {
            // .NET 5+ has this additional overload as an entry point.

            bool BaseValidate()
            {
                return base.Validate(metadata, key, model, alwaysValidateAtTopLevel, container);
            }

            return ValidateInternal(metadata, key, model, BaseValidate);
        }

        private bool ValidateInternal(ModelMetadata? metadata, string? key, object? model, Func<bool> continuation)
        {
            MvcValidationHelper.SetRootMetadata(Context, metadata);

            // Store and remove any implicit required messages.
            // Later we'll re-add those that are still relevant.
            List<KeyValuePair<ModelStateEntry, ModelError>> requiredErrorsNotHandledByValidot = MvcValidationHelper.RemoveImplicitRequiredErrors(Context);

            bool result = continuation();

            // Re-add errors that we took out if Validot didn't add a key.
            MvcValidationHelper.ReApplyUnhandledImplicitRequiredErrors(requiredErrorsNotHandledByValidot);

            // Remove duplicates. This can happen if someone has implicit child validation turned on and also adds an explicit child validator.
            MvcValidationHelper.RemoveDuplicateModelStateEntries(Context);

            return result;
        }
    }
}
namespace Validot.AspNetCore.Internal
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Validot.AspNetCore;
    using Validot.DependencyInjection;
    using Validot.Results;

    /// <summary>
    /// Validot's implementation of an ASP.NET Core <see cref="IModelValidator"/>.
    /// </summary>
    /// <remarks>Based on: https://github.com/FluentValidation/FluentValidation.AspNetCore</remarks>
    internal sealed class ValidotModelValidator : IModelValidator
    {
        private readonly ValidotMvcOptions _options;
        private readonly ModelMetadata _modelMetadata;

        private readonly string? _defaultLanguage;
        private readonly IValidotValidator? _validator;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidotModelValidatorProvider"/>.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="validotValidatorProvider"></param>
        /// <param name="modelMetadata"></param>
        public ValidotModelValidator(IOptions<ValidotMvcOptions> options,
                                     IValidotValidatorProvider validotValidatorProvider,
                                     ModelMetadata modelMetadata)
        {
            _options = options.Value;
            _modelMetadata = modelMetadata ?? throw new ArgumentNullException(nameof(modelMetadata));

            _defaultLanguage = string.IsNullOrWhiteSpace(_options.DefaultLanguage)
                ? null
                : _options.DefaultLanguage.Trim();

            _validator = validotValidatorProvider.GetValidator(_modelMetadata.ModelType);
        }

        /// <inheritdoc/>
        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext modelValidationContext)
        {
            if (_validator is null || // Skip if no validator.
                modelValidationContext.Model is null || // // Skip if there's nothing to process.
                ShouldSkip(modelValidationContext)) // Skip when other skip conditions are satisfied.
            {
                yield break;
            }

            IValidationResult result = _validator.Validate(modelValidationContext.Model, _options.FailFast);

            if (!result.AnyErrors)
            {
                yield break;
            }

            string? contextLanguage = GetContextLanguage(modelValidationContext);

            IReadOnlyDictionary<string, IReadOnlyList<string>> messageMap = GetMessageMap(result, contextLanguage);

            foreach ((string path, IReadOnlyList<string> errorMessages) in messageMap)
            {
                foreach (string errorMessage in errorMessages)
                {
                    yield return new ModelValidationResult(path, errorMessage);
                }
            }
        }

        private static string? GetContextLanguage(ModelValidationContext modelValidationContext)
        {
            IValidationResultLanguageProvider? validationResultLangaugeProvider =
                modelValidationContext.ActionContext.HttpContext.RequestServices.GetService<IValidationResultLanguageProvider>();

            return validationResultLangaugeProvider?.GetLangauge(modelValidationContext);
        }

        private IReadOnlyDictionary<string, IReadOnlyList<string>> GetMessageMap(IValidationResult result, string? contextLanguage = null)
        {
            contextLanguage = contextLanguage?.Trim();

            if (string.IsNullOrEmpty(contextLanguage) &&
                result.TranslationNames.Contains(contextLanguage))
            {
                return result.GetTranslatedMessageMap(contextLanguage);
            }

            return _defaultLanguage is null || !result.TranslationNames.Contains(_defaultLanguage)
                ? result.MessageMap
                : result.GetTranslatedMessageMap(_defaultLanguage);
        }

        private bool ShouldSkip(ModelValidationContext modelValidationContext)
        {
            // If implicit validation is disabled, then we want to only validate the root object.
            if (!_options.ImplicitValidationEnabled)
            {
                ModelMetadata? rootMetadata = GetRootMetadata(modelValidationContext);

                // We should always have root metadata, so this should never happen...
                if (rootMetadata is null)
                {
                    return true;
                }

                ModelMetadata modelMetadata = modelValidationContext.ModelMetadata;

                // Careful when handling properties.
                // If we're processing a property of our root object,
                // then we always skip if implicit validation is disabled
                // However if our root object *is* a property (because of [BindProperty])
                // then this is OK to proceed.
                if (modelMetadata.MetadataKind is ModelMetadataKind.Property)
                {
                    if (!ReferenceEquals(rootMetadata, modelMetadata))
                    {
                        // The metadata for the current property is not the same as the root metadata
                        // This means we're validating a property on a model, so we want to skip.
                        return true;
                    }
                }

                // If we're handling a type, we need to make sure we're handling the root type.
                // When MVC encounters child properties, it will set the MetadataKind to Type,
                // so we can't use the MetadataKind to differentiate the root from the child property.
                // Instead check if our cached root metadata is the same.
                // If they're not, then it means we're handling a child property, so we should skip
                // validation if implicit validation is disabled
                else if (modelMetadata.MetadataKind is ModelMetadataKind.Type)
                {
                    // If implicit validation of root collection elements is enabled then we
                    // do want to validate the type if it matches the element type of the root collection
                    if (_options.ImplicitRootCollectionElementValidationEnabled &&
                        IsRootCollectionElementType(rootMetadata, modelMetadata.ModelType))
                    {
                        return false;
                    }

                    if (!ReferenceEquals(rootMetadata, modelMetadata))
                    {
                        // The metadata for the current type is not the same as the root metadata
                        // This means we're validating a child element of a collection or sub property.
                        // Skip it as implicit validation is disabled.
                        return true;
                    }
                }
                else if (modelMetadata.MetadataKind is ModelMetadataKind.Parameter)
                {
                    // If we're working with record types then metadata kind will always be parameter.
                    if (!ReferenceEquals(rootMetadata, modelMetadata))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the metadata object for the root object being validated.
        /// </summary>
        /// <param name="mvContext">MVC Validation context</param>
        /// <returns>Metadata instance.</returns>
        private static ModelMetadata? GetRootMetadata(ModelValidationContext mvContext)
        {
            return MvcValidationHelper.GetRootMetadata(mvContext);
        }

        private static bool IsRootCollectionElementType(ModelMetadata rootMetadata, Type modelType)
        {
            if (!rootMetadata.IsEnumerableType)
            {
                return false;
            }

            return modelType == rootMetadata.ElementType;
        }
    }
}
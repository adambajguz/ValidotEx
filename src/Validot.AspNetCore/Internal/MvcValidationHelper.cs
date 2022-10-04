namespace Validot.AspNetCore.Internal
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    /// <summary>
    /// Utilities for working around limitations of the MVC validation api.
    /// Used by <see cref="ValidotValidationVisitor"/>
    /// </summary>
    /// <remarks>Based on: https://github.com/FluentValidation/FluentValidation.AspNetCore</remarks>
    internal static class MvcValidationHelper
    {
        private const string RootMetadataKey = "_Validot_ROOT_METADATA";

        internal static void SetRootMetadata(ActionContext context, ModelMetadata? metadata)
        {
            context.HttpContext.Items[RootMetadataKey] = metadata;
        }

        internal static ModelMetadata? GetRootMetadata(ModelValidationContext context)
        {
            if (context.ActionContext.HttpContext.Items.TryGetValue(RootMetadataKey, out object? rootMetadata))
            {
                return rootMetadata as ModelMetadata;
            }

            return null;
        }

        internal static List<KeyValuePair<ModelStateEntry, ModelError>> RemoveImplicitRequiredErrors(ActionContext actionContext)
        {
            // This is all to work around the default "Required" messages.
            List<KeyValuePair<ModelStateEntry, ModelError>> requiredErrorsNotHandledByValidot = new();

            foreach (KeyValuePair<string, ModelStateEntry> entry in actionContext.ModelState)
            {
                List<ModelError> errorsToModify = new();

                if (entry.Value.ValidationState == ModelValidationState.Invalid)
                {
                    foreach (ModelError err in entry.Value.Errors)
                    {
                        if (err.ErrorMessage.StartsWith(ValidotBindingMetadataProvider.Prefix))
                        {
                            errorsToModify.Add(err);
                        }
                    }

                    foreach (ModelError err in errorsToModify)
                    {
                        entry.Value.Errors.Clear();
                        entry.Value.ValidationState = ModelValidationState.Unvalidated;

                        ModelError modelError = new(err.ErrorMessage.Replace(ValidotBindingMetadataProvider.Prefix, string.Empty));

                        requiredErrorsNotHandledByValidot.Add(new KeyValuePair<ModelStateEntry, ModelError>(entry.Value, modelError));
                    }
                }
            }

            return requiredErrorsNotHandledByValidot;
        }

        internal static void ReApplyUnhandledImplicitRequiredErrors(List<KeyValuePair<ModelStateEntry, ModelError>> requiredErrorsNotHandledByFv)
        {
            foreach (KeyValuePair<ModelStateEntry, ModelError> pair in requiredErrorsNotHandledByFv)
            {
                if (pair.Key.ValidationState != ModelValidationState.Invalid)
                {
                    pair.Key.Errors.Add(pair.Value);
                    pair.Key.ValidationState = ModelValidationState.Invalid;
                }
            }
        }

        internal static void RemoveDuplicateModelStateEntries(ActionContext actionContext)
        {
            foreach (KeyValuePair<string, ModelStateEntry> entry in actionContext.ModelState)
            {
                if (entry.Value.ValidationState == ModelValidationState.Invalid)
                {
                    HashSet<string> existing = new();

                    foreach (ModelError? err in entry.Value.Errors.ToList())
                    {
                        //ToList to create a copy so we can remove from the original
                        if (existing.Contains(err.ErrorMessage))
                        {
                            entry.Value.Errors.Remove(err);
                        }
                        else
                        {
                            existing.Add(err.ErrorMessage);
                        }
                    }
                }
            }
        }
    }
}
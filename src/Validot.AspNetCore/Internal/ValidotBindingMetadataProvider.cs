namespace Validot.AspNetCore.Internal
{
    using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

    /// <summary>
    /// Validot specific implementation of <see cref="IBindingMetadataProvider"/>.
    /// </summary>
    /// <remarks>Based on: https://github.com/FluentValidation/FluentValidation.AspNetCore</remarks>
    internal sealed class ValidotBindingMetadataProvider : IBindingMetadataProvider
    {
        public const string Prefix = "_Validot_REQUIRED|";

        /// <summary>
        /// If we're validating a non-nullable value type then
        /// MVC will automatically add a "Required" error message.
        /// We prefix these messages with a placeholder, so we can identify and remove them
        /// during the validation process.
        /// <see cref="ValidotValidationVisitor"/>
        /// <see cref="MvcValidationHelper.RemoveImplicitRequiredErrors"/>
        /// <see cref="MvcValidationHelper.ReApplyUnhandledImplicitRequiredErrors"/>
        /// </summary>
        /// <param name="context"></param>
        public void CreateBindingMetadata(BindingMetadataProviderContext context)
        {
            if (context.Key.MetadataKind is ModelMetadataKind.Property)
            {
                if (context.BindingMetadata.ModelBindingMessageProvider is { } modelBindingMessageProvider)
                {
                    Func<string, string> original = context.BindingMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor;

                    modelBindingMessageProvider.SetValueMustNotBeNullAccessor(s => Prefix + original(s));
                }
                else
                {
                    throw new InvalidOperationException("ModelBindingMessageProvider is null");
                }
            }
        }
    }
}
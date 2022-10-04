namespace Validot.AspNetCore
{
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Validot.Results;

    /// <summary>
    /// <see cref="IValidationResult"/> error messages translation language provider.
    /// </summary>
    public interface IValidationResultLanguageProvider
    {
        /// <summary>
        /// Gets the language to be used for translating <see cref="IValidationResult"/> error messages.
        /// </summary>
        /// <param name="modelValidationContext"></param>
        string? GetLangauge(ModelValidationContext modelValidationContext);
    }
}

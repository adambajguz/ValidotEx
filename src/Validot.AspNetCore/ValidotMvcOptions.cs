namespace Validot.AspNetCore
{
    using Validot.Results;

    /// <summary>
    /// Validot options for ASP.NET Core MVC integration.
    /// </summary>
    public sealed class ValidotMvcOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether implicit validation is enabled (default: true).
        /// </summary>
        public bool ImplicitValidationEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether implicit validation of root collection elements is enabled.
        /// </summary>
        public bool ImplicitRootCollectionElementValidationEnabled { get; set; } = true;

        /// <summary>
        /// If true, the validation process will stop after detecting the first error. Otherwise, full validation is performed (default: false).
        /// </summary>
        public bool FailFast { get; set; }

        /// <summary>
        /// Default error message translation language (default: English).
        /// If null and no context language is set, <see cref="IValidationResult.MessageMap"/> is used instead of <see cref="IValidationResult.GetTranslatedMessageMap(string)"/>.
        /// </summary>
        public string? DefaultLanguage { get; set; } = "English";
    }
}

namespace Validot.DependencyInjection.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Validot;
    using Validot.Errors;
    using Validot.Results;

    internal static class ValidotErrorRegistryProvider
    {
        private readonly struct FieldNames
        {
            /// <summary>
            /// _resultErrors field name.
            /// </summary>
            public const string ResultErrors = "_resultErrors";

            /// <summary>
            /// _errorRegistry field name.
            /// </summary>
            public const string ErrorRegistry = "_errorRegistry";
        }

        /// <summary>
        /// Get error registry from <paramref name="validator"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IReadOnlyDictionary<string, IReadOnlyList<IError>> Get<T>(IValidator<T> validator)
        {
            ArgumentNullException.ThrowIfNull(validator);

            try
            {
                return Get(validator.Template);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to ger error registry for {validator.GetType()}.", ex);
            }
        }

        /// <summary>
        /// Get error registry from <paramref name="validationResult"/>.
        /// </summary>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static IReadOnlyDictionary<string, IReadOnlyList<IError>> Get(IValidationResult validationResult)
        {
            ArgumentNullException.ThrowIfNull(validationResult);

            Type validationResultType = validationResult.GetType();

            FieldInfo resultErrorsField = validationResultType.GetField(FieldNames.ResultErrors, BindingFlags.Instance | BindingFlags.NonPublic) ??
                throw new InvalidOperationException($"Field '{FieldNames.ResultErrors}' not found in '{validationResultType}'.");

            FieldInfo errorRegistryField = validationResultType.GetField(FieldNames.ErrorRegistry, BindingFlags.Instance | BindingFlags.NonPublic) ??
                throw new InvalidOperationException($"Field '{FieldNames.ErrorRegistry}' not found in '{validationResultType}'.");

            IReadOnlyDictionary<string, List<int>> resultErrors = (IReadOnlyDictionary<string, List<int>>)(resultErrorsField.GetValue(validationResult) ??
                throw new InvalidOperationException($"Field '{FieldNames.ResultErrors}' in '{validationResultType}' not expected to be null."));

            IReadOnlyDictionary<int, IError> errorRegistry = (IReadOnlyDictionary<int, IError>)(errorRegistryField.GetValue(validationResult) ??
                throw new InvalidOperationException($"Field '{FieldNames.ErrorRegistry}' in '{validationResultType}' not expected to be null."));

            Dictionary<string, IReadOnlyList<IError>> output = new();
            foreach ((string key, List<int> value) in resultErrors)
            {
                List<IError> errors = new();

                foreach (int errorId in value)
                {
                    errors.Add(errorRegistry[errorId]);
                }

                output.Add(key, errors);
            }

            return output;
        }
    }
}

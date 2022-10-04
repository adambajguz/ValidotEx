namespace WebApplicationExample.Contracts.Account.ChangePasswordByEmail
{
    using System;
    using Validot;
    using Validot.Factory;
    using Validot.Settings;
    using WebApplicationExample.Extensions;

    /// <summary>
    /// Validator for <see cref="ChangeAccountPasswordByEmailRequest.Body"/>.
    /// </summary>
    public sealed class ChangeAccountPasswordByEmailRequest_BodyValidator : ISettingsHolder, ISpecificationHolder<ChangeAccountPasswordByEmailRequest.Body>
    {
        private const string PasswordsMustNotEqual = "Text.PasswordsMustNotEqual";

        /// <inheritdoc/>
        public Func<ValidatorSettings, ValidatorSettings> Settings { get; }

        /// <inheritdoc/>
        public Specification<ChangeAccountPasswordByEmailRequest.Body> Specification { get; }

        /// <summary>
        /// Initializes an instance of <see cref="ChangeAccountPasswordByEmailRequest_BodyValidator"/>.
        /// </summary>
        public ChangeAccountPasswordByEmailRequest_BodyValidator()
        {
            Settings = s => s
                .ApplySharedSettings()
                .WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["English"] = new Dictionary<string, string>()
                    {
                        [PasswordsMustNotEqual] = "Current and new password must not be the same."
                    },
                    ["Polish"] = new Dictionary<string, string>()
                    {
                        [PasswordsMustNotEqual] = "Obecne i nowe hasło nie mogą być takie same."
                    }
                });

            Specification = m => m
                .Member(x => x.CurrentPassword, currentPassword => currentPassword
                    .NotEmpty()
                    .MinLength(8)
                    .MaxLength(64)
                )
                .Member(x => x.NewPassword, currentPassword => currentPassword
                    .NotEmpty()
                    .MinLength(8)
                    .MaxLength(64)
                )
                .Rule(x => x.CurrentPassword != x.NewPassword)
                .WithMessage(PasswordsMustNotEqual);
        }
    }
}

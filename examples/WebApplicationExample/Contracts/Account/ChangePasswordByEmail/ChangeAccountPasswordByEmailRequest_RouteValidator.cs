namespace WebApplicationExample.Contracts.Account.ChangePasswordByEmail
{
    using Validot;
    using Validot.Factory;
    using Validot.Settings;
    using WebApplicationExample.Extensions;

    /// <summary>
    /// Validator for <see cref="ChangeAccountPasswordByEmailRequest.Route"/>.
    /// </summary>
    public sealed class ChangeAccountPasswordByEmailRequest_RouteValidator : ISettingsHolder, ISpecificationHolder<ChangeAccountPasswordByEmailRequest.Route>
    {
        /// <inheritdoc/>
        public Func<ValidatorSettings, ValidatorSettings> Settings { get; }

        /// <inheritdoc/>
        public Specification<ChangeAccountPasswordByEmailRequest.Route> Specification { get; }

        /// <summary>
        /// Initializes an instance of <see cref="ChangeAccountPasswordByEmailRequest_RouteValidator"/>.
        /// </summary>
        public ChangeAccountPasswordByEmailRequest_RouteValidator()
        {
            Settings = s => s
                .ApplySharedSettings();

            Specification = m => m
                .Member(x => x.Email, email => email
                    .NotWhiteSpace()
                    .Email()
                    .MaxLength(254)
                );
        }
    }
}

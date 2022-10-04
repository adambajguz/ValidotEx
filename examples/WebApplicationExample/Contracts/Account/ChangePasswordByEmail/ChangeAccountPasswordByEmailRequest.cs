namespace WebApplicationExample.Contracts.Account.ChangePasswordByEmail
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Reset account password by identifier.
    /// </summary>
    public static class ChangeAccountPasswordByEmailRequest
    {
        /// <summary>
        /// Request route.
        /// </summary>
        public sealed record Route
        {
            /// <summary>
            /// Account identifier.
            /// </summary>
            [FromRoute(Name = "email")]
            public string? Email { get; init; }
        }

        /// <summary>
        /// Request body.
        /// </summary>
        public sealed record Body
        {
            /// <summary>
            /// Current password.
            /// </summary>
            public string? CurrentPassword { get; init; }

            /// <summary>
            /// New password.
            /// </summary>
            public string? NewPassword { get; init; }
        }
    }
}

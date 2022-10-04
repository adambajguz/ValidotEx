namespace WebApplicationExample.Controllers
{
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using WebApplicationExample.Contracts.Account.ChangePasswordByEmail;

    /// <summary>
    /// Account controller.
    /// </summary>
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger _logger;

        public AccountController(ILogger<AccountController> logger) : base()
        {
            _logger = logger;
        }

        /// <summary>
        /// Change password for an account with given email.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPatch("by-email/{email}/password/change")]
        public void ChangePasswordByEmail([FromRoute] ChangeAccountPasswordByEmailRequest.Route route,
                                          [FromBody] ChangeAccountPasswordByEmailRequest.Body body)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ChangePasswordByEmail request invalid [{@Errors}]", JsonSerializer.Serialize(ModelState, options: new JsonSerializerOptions { WriteIndented = true }));
            }

            _logger.LogInformation("ChangePasswordByEmail called [{@Route} | {@Body}]", route, body);
        }
    }
}

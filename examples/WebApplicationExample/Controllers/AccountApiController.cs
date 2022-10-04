namespace WebApplicationExample.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using WebApplicationExample.Contracts.Account.ChangePasswordByEmail;

    /// <summary>
    /// Account controller.
    /// </summary>
    [ApiController]
    [Route("api/account")]
    public class AccountApiController : ControllerBase
    {
        private readonly ILogger _logger;

        public AccountApiController(ILogger<AccountApiController> logger) : base()
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
            _logger.LogInformation("ChangePasswordByEmail called [{@Route} | {@Body}]", route, body);
        }
    }
}

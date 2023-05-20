using Microsoft.AspNetCore.Mvc;
using UserWebAPI.Interfaces;
using UserWebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatorController : ControllerBase
    {
        private readonly IAuthenticatorService _authenticatorService;
        private readonly ILogger<AuthenticatorController> _logger;

        public AuthenticatorController(IAuthenticatorService authenticatorService, 
            ILogger<AuthenticatorController> logger)
        {
            _authenticatorService = authenticatorService;
            _logger = logger;
        }

        // POST api/<AuthenticatorController>
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            GeneralResponse result = await _authenticatorService.Login(loginRequest);

            return StatusCode((int) result.Code!, result);
        }
    }
}

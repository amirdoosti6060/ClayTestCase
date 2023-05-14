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

        public AuthenticatorController(IAuthenticatorService authenticatorService)
        {
            _authenticatorService = authenticatorService;
        }

        // GET: api/<AuthenticatorController>
        [HttpGet]
        public GeneralResponse Get()
        {
            GeneralResponse response = new GeneralResponse
            {
                ErrorCode = StatusCodes.Status200OK,
                Data = "healthy"
            };

            return response;
        }

        // POST api/<AuthenticatorController>
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            GeneralResponse result = await _authenticatorService.Login(loginRequest);

            return StatusCode((int) result.ErrorCode!, result);
        }
    }
}

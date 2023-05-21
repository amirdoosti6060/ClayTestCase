using DoorWebAPI.Interfaces;
using DoorWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DoorWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoorController : ControllerBase
    {
        private readonly IDoorService _doorService;

        public DoorController(IDoorService doorService)
        {
            _doorService = doorService;
        }

        // GET: api/<DoorController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _doorService.GetAll();

            return StatusCode((int)response.Code!, response);
        }

        // GET api/<DoorController>/5
        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var response = await _doorService.Get(id);

            return StatusCode((int)response.Code!, response);
        }

        // POST api/<DoorController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddUpdateDoorRequest addUpdateDoorRequest)
        {
            var response = await _doorService.Add(addUpdateDoorRequest);

            return StatusCode((int)response.Code!, response);
        }

        // PUT api/<DoorController>/5
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Put(long id, [FromBody] AddUpdateDoorRequest addUpdateUserRequest)
        {
            var response = await _doorService.Update(id, addUpdateUserRequest);

            return StatusCode((int)response.Code!, response);
        }

        // DELETE api/<DoorController>/5
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _doorService.Delete(id);

            return StatusCode((int)response.Code!, response);
        }

        // GET api/<DoorController>
        [HttpGet("Unlock/{doorid:long}")]
        public async Task<IActionResult> Unlock(long doorid)
        {
            UserInfo? userInfo = GetCurrentUserInfo(HttpContext);

            var response = await _doorService.Unlock(userInfo!, doorid);

            return StatusCode((int)response.Code!, response);
        }
        
        private UserInfo? GetCurrentUserInfo(HttpContext httpContext)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new UserInfo
                {
                    Id = Convert.ToInt64(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value),
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value!,
                    FullName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value!,
                    Role = userClaims.FirstOrDefault(o => o.Type == "Role")?.Value!
                };
            }

            return null;
        }
    }
}

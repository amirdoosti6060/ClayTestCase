using Microsoft.AspNetCore.Mvc;
using UserWebAPI.Interfaces;
using UserWebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _userService.GetAll();

            return StatusCode((int)response.ErrorCode!, response);
        }

        // GET api/<UserController>/5
        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var response = await _userService.Get(id);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddUserRequest addUserRequest)
        {
            User user = new User
            {
                Email = addUserRequest.Email,
                Password = addUserRequest.Password,
                FullName = addUserRequest.FullName,
                Role = addUserRequest.Role
            };

            var response = await _userService.Add(user);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Put(long id, [FromBody] UpdateUserRequest updateUserRequest)
        {
            User user = new User
            {
                Email = updateUserRequest.Email,
                Password = updateUserRequest.Password,
                FullName = updateUserRequest.FullName,
                Role = updateUserRequest.Role
            };

            var response = await _userService.Update(id, user);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _userService.Delete(id);

            return StatusCode((int) response.ErrorCode!, response);
        }
    }
}

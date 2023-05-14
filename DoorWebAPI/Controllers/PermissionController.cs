using DoorWebAPI.Interfaces;
using DoorWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DoorWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        // GET: api/<PermissionController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _permissionService.GetAll();

            return StatusCode((int)response.ErrorCode!, response);
        }

        // GET api/<PermissionController>/5
        [HttpGet("{permid:long}")]
        public async Task<IActionResult> Get(long permid)
        {
            var response = await _permissionService.Get(permid);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // GET api/<PermissionController>/1/administrator
        [HttpGet("{doorid:long}/{role}")]
        public async Task<IActionResult> Get(long doorid, string role)
        {
            var response = await _permissionService.Get(doorid, role);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // GET: api/<PermissionController>/GetAllByDoor/1
        [HttpGet("GetAllByDoor/{doorid:long}")]
        public async Task<IActionResult> GetAllByDoor(long doorid)
        {
            var response = await _permissionService.GetAllByDoor(doorid);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // GET: api/<PermissionController>/GetAllByRole/1
        [HttpGet("GetAllByRole/{role}")]
        public async Task<IActionResult> GetAllByRole(string role)
        {
            var response = await _permissionService.GetAllByRole(role);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // POST api/<PermissionController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddPermissionRequest addPermRequest)
        {
            Permission perm = new Permission
            {
                DoorId = addPermRequest.DoorId,
                Role = addPermRequest.Role
            };

            var response = await _permissionService.Add(perm);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // DELETE api/<PermissionController>/5
        [HttpDelete("{permid:long}")]
        public async Task<IActionResult> Delete(long permid)
        {
            var response = await _permissionService.Delete(permid);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // DELETE api/<PermissionController>/1/user
        [HttpDelete("{doorid:long}/{role}")]
        public async Task<IActionResult> Delete(long doorid, string role)
        {
            var response = await _permissionService.Delete(doorid, role);

            return StatusCode((int)response.ErrorCode!, response);
        }
    }
}

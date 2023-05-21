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
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        // GET: api/<PermissionController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetPermissionRequest getPermissionRequest)
        {
            var response = await _permissionService.Get(getPermissionRequest);

            return StatusCode((int)response.Code!, response);
        }

        // GET api/<PermissionController>/5
        [HttpGet("{permid:long}")]
        public async Task<IActionResult> Get(long permid)
        {
            var response = await _permissionService.Get(permid);

            return StatusCode((int)response.Code!, response);
        }

        // POST api/<PermissionController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddPermissionRequest addPermRequest)
        {
            var response = await _permissionService.Add(addPermRequest);

            return StatusCode((int)response.Code!, response);
        }

        // DELETE api/<PermissionController>/5
        [HttpDelete("{permid:long}")]
        public async Task<IActionResult> DeleteById(long permid)
        {
            var response = await _permissionService.Delete(permid);

            return StatusCode((int)response.Code!, response);
        }

        // DELETE api/<PermissionController>/1/user
        [HttpDelete("{doorid:long}/{role}")]
        public async Task<IActionResult> Delete(long doorid, string role)
        {
            var response = await _permissionService.Delete(doorid, role);

            return StatusCode((int)response.Code!, response);
        }
    }
}

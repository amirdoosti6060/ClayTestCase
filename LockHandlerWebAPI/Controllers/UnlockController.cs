using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LockHandlerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnlockController : ControllerBase
    {
        // POST api/<LockController>
        [HttpGet]
        public IActionResult Get(string? hardwareid)
        {
            if (hardwareid == null)
                return BadRequest("Invalid hardware ID!");
            else if (hardwareid.ToLower().StartsWith("o-"))
                return Ok("Unlocked successfully.");
            else if (hardwareid.ToLower().StartsWith("b-"))
                return BadRequest("Lock has problem!");
            else if (hardwareid.ToLower().StartsWith("d-"))
            {
                Task.Delay(3000);
                return StatusCode(StatusCodes.Status408RequestTimeout, "Door respond slowly!");
            }

            return NotFound("Lock not found!");
        }
    }
}

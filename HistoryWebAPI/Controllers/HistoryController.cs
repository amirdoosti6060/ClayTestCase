using HistoryWebAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HistoryWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        // GET api/<HistoryController>/GetByDate/2022/2/23
        [HttpGet("{year:int}/{month:int}/{day:int}")]
        public async Task<IActionResult> GetByDate(int year, int month, int day)
        {
            var response = await _historyService.GetByDate(year, month, day);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // GET api/<HistoryController>/GetByDoorId/1
        [HttpGet("{doorId:long}")]
        public async Task<IActionResult> GetByDoorId(long doorId)
        {
            var response = await _historyService.GetByDoorId(doorId);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // GET api/<HistoryController>/GetByRole/user
        [HttpGet("{role}")]
        public async Task<IActionResult> GetByRole(string role)
        {
            var response = await _historyService.GetByRole(role);

            return StatusCode((int)response.ErrorCode!, response);
        }

        // GET api/<HistoryController>/GetByUserId/1
        [HttpGet("{userId:long}")]
        public async Task<IActionResult> GetByUserId(long userId)
        {
            var response = await _historyService.GetByUserId(userId);

            return StatusCode((int)response.ErrorCode!, response);
        }
    }
}

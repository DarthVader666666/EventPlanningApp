using EventPlanning.Bll;
using Microsoft.AspNetCore.Mvc;

namespace ReactApp1.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly BllService _bllService;

        public EventsController(BllService bllService)
        {
            _bllService = bllService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok(_bllService.GetGreeting());
        }
    }
}

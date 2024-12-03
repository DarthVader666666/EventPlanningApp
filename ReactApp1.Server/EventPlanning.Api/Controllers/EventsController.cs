using Microsoft.AspNetCore.Mvc;

namespace ReactApp1.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        public EventsController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok(Enumerable.Empty<object>());
        }
    }
}

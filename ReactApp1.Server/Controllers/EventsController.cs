using EventPlanning.Api.Models;
using EventPlanning.Bll;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ReactApp1.Server.Controllers
{
    [ApiController]
    [EnableCors("AllowClient")]
    public class EventsController : ControllerBase
    {
        private readonly BllService _bllService;

        public EventsController(BllService bllService)
        {
            _bllService = bllService;
        }

        [HttpGet]
        [Route("api/[controller]")]
        public async Task<IActionResult> Index()
        {
            return Ok(_bllService.GetGreeting());
        }

        [HttpGet]
        [Route("api/[controller]/{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            return Ok(_bllService.GetGreeting());
        }

        //[HttpPost]
        //[Route("[controller]/[action]")]
        //public async Task<IActionResult> Login([FromBody] UserLogInModel userLoginModel)
        //{
        //    return Ok("LogIn Post worked");
        //}
    }
}

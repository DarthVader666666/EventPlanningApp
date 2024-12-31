using AutoMapper;
using Azure.Communication.Email;
using EventPlanning.Api.Models;
using EventPlanning.Bll.Interfaces;
using EventPlanning.Bll.Services;
using EventPlanning.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace EventPlanning.Api.Controllers
{
    [ApiController]
    [EnableCors("AllowClient")]
    public class EventsController : ControllerBase
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserEvent> _userEventRepository;
        private readonly EmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public EventsController(IRepository<Event> eventRepository, IRepository<User> userRepository, IRepository<UserEvent> userEventRepository,
            IConfiguration configuration, IMapper mapper, EmailSender emailSender)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _userEventRepository = userEventRepository;
            _configuration = configuration;
            _emailSender = emailSender;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("api/[controller]")]
        public async Task<IActionResult> Index()
        {
            var events = await _eventRepository.GetListAsync() ?? throw new NullReferenceException("Recieved null parameter events");
            var mappedEvents = _mapper.Map<IEnumerable<Event>, IEnumerable<EventIndexModel>>(events!);
            return Ok(mappedEvents);
        }

        [HttpGet]
        [Route("api/[controller]/{eventId:int}")]
        public async Task<EventIndexModel> GetEvent([FromRoute] int? eventId)
        {
            var eventEntity = await _eventRepository.GetAsync(eventId) ?? throw new NullReferenceException("Recieved null parameter eventEntity");
            var mappedEvent = _mapper.Map<Event, EventIndexModel>(eventEntity!);

            return mappedEvent;
        }

        [HttpPost]
        [Route("api/[controller]/create")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Create([FromBody] EventCreateModel model)
        {
            try
            {
                var newEvent = _mapper.Map<EventCreateModel, Event>(model);
                await _eventRepository.CreateAsync(newEvent);
            }
            catch (SqlException)
            {
                return BadRequest("Error while creating event");
            }

            return Redirect($"{_configuration["ClientUrl"]}/");
        }

        [HttpDelete]
        [Route("api/[controller]/remove")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Remove([FromBody] EventIndexModel model)
        {
            try
            {
                await _eventRepository.DeleteAsync(model.EventId);
            }
            catch (SqlException)
            {
                return BadRequest("Error while creating event");
            }

            return Redirect($"{_configuration["ClientUrl"]}/");
        }

        [HttpPost]
        [Route("api/[controller]/participate")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Participate([FromBody] EventConfirm eventConfirmModel)
        {
            if (eventConfirmModel == null)
            {
                return BadRequest("eventConfirmModel is null");
            }

            var user = await _userRepository.GetAsync(eventConfirmModel.Email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            UserEvent userEvent = await _userEventRepository.GetAsync(new Tuple<int?, int?>(user.UserId, eventConfirmModel.EventId));

            if (userEvent != null && (userEvent.EmailConfirmed ?? true))
            {
                return Ok(new { message = "You already participate in Event" });
            }

            userEvent = new UserEvent()
            {
                UserId = (int)user.UserId!,
                EventId = (int)eventConfirmModel.EventId!
            };

            if (!await _userEventRepository.ExistsAsync(userEvent))
            {
                await _userEventRepository.CreateAsync(userEvent);
            }

            var url = $"<button>" +
                $"<a href='{_configuration["ClientUrl"]}/api/events/confirm?userId={userEvent.UserId}&eventId={userEvent.EventId}&email={eventConfirmModel.Email}' " +
                $"style=\"text-decoration: none; color: black\">" +
                $"Confirm Participation" +
                $"</a>" +
                $"</button>";

            var result = await _emailSender.SendEmailAsync(eventConfirmModel.Email ?? throw new ArgumentNullException("Email", "Email value is null"), "Thank you! Event participation confirmed!", url);

            if (result?.Value.Status == EmailSendStatus.Succeeded)
            {
                return Ok(new { message = "Email sent" });
            }
            else
            {
                return BadRequest("Error while sending email");
            }
        }

        [HttpGet]
        [Route("api/[controller]/confirm")]
        public async Task<IActionResult> Confirm([FromQuery] int userId, [FromQuery] int eventId, [FromQuery] string email)
        {
            var user = await _userRepository.GetAsync(email);

            if (user == null)
            {
                return Redirect($"{_configuration["ClientUrl"]}/confirm?success=false&message=User%20was%20not");
            }

            var userEvent = await _userEventRepository.GetAsync(new Tuple<int?, int?>(userId, eventId));

            if (userEvent == null)
            {
                return Redirect($"{_configuration["ClientUrl"]}/confirm?success=false&message=User%20has%20not%20been%20binded%20to%20Event&email={email}&password={user.Password}");
            }

            if (userEvent.EmailConfirmed ?? false)
            {
                return Redirect($"{_configuration["ClientUrl"]}/confirm?success=false&message=User%20already%20participates%20in%20Event&email={email}&password={user.Password}");
            }

            userEvent.EmailConfirmed = true;
            await _userEventRepository.UpdateAsync(userEvent);

            var updatedEvent = await _eventRepository.GetAsync(eventId);

            if (updatedEvent != null)
            {
                var amount = updatedEvent.AmountOfVacantPlaces > 0 ? (updatedEvent.AmountOfVacantPlaces - 1) : 0;
                updatedEvent.AmountOfVacantPlaces = amount;
                await _eventRepository.UpdateAsync(updatedEvent);
            }
            else
            {
                return Redirect($"{_configuration["ClientUrl"]}/confirm?success=false&message=Event%20could%20not%20be%20updated");
            }

            return Redirect($"{_configuration["ClientUrl"]}/confirm?success=true&email={email}&password={user.Password}");
        }
    }
}

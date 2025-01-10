using AutoMapper;
using EventPlanning.Bll.Interfaces;
using EventPlanning.Data.Entities;
using EventPlanning.Api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventPlanning.Bll.Services;
using Azure.Communication.Email;

namespace EventPlanning.Api.Controllers
{
    [EnableCors("AllowClient")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly CryptoService _cryptoService;
        private readonly EmailSender _emailSender;

        public AuthorizationController(
            IRepository<User> userRepository, IRepository<Role> roleRepository, IMapper mapper, 
            IConfiguration configuration, CryptoService cryptoService, EmailSender emailSender)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _configuration = configuration;
            _cryptoService = cryptoService;
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> LogIn([FromBody] UserLogInModel userLogIn)
        {
            var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(jwtToken))
            {
                var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
                var user_name = jwtSecurityToken?.Claims?.FirstOrDefault(x => x.Type.Contains("identity/claims/name"))?.Value;
                var role = jwtSecurityToken?.Claims?.FirstOrDefault(x => x.Type.Contains("identity/claims/role"))?.Value;
                var user = await _userRepository.GetAsync(user_name);

                if (user == null) 
                {
                    NotFound(new { errorText = "User does not exist." });
                }

                return Ok(new LogInResponseModel
                {
                    access_token = jwtToken,
                    user_name = _cryptoService.Decrypt(user_name),
                    role = role
                });
            }

            var identity = await GetIdentity(userLogIn);

            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid email or password." });
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: "Test",
                    audience: "Test",
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(20d)),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_configuration["SecurityKey"] ?? throw new ArgumentNullException("SecurityKey", "SecurityKey can't be null"))), SecurityAlgorithms.HmacSha256)
                    );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);            

            var response = new LogInResponseModel
            {
                access_token = encodedJwt,
                user_name = _cryptoService.Decrypt(identity.Name),
                role = identity.RoleClaimType
            };

            HttpContext.Session.SetString("access_token", response.access_token);

            return Ok(response);
        }

        [HttpPost]
        [Route("api/[controller]/register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel userRegister)
        {
            if (userRegister.Password == null)
            {
                return BadRequest("Password is null");
            }

            if (userRegister.Email == null)
            {
                return BadRequest("Email is null");
            }

            if (await DoesUserExist(_cryptoService.Encrypt(userRegister.Email)))
            {
                return BadRequest(new { errorText = "User with this email already exists." });
            }

            var url = $"<button>" +
                $"<a href='{_configuration["ClientUrl"]}/api/authorization/confirm?" +
                $"encryptedEmail={_cryptoService.Encrypt(userRegister.Email)}&encryptedPassword={_cryptoService.Encrypt(userRegister.Password)}" +
                $"&firstName={userRegister.FirstName}&lastName={userRegister.LastName}' " +
                $"style=\"text-decoration: none; color: black\">" +
                $"Confirm Registration" +
                $"</a>" +
                $"</button>";

            var result = await _emailSender.SendEmailAsync(userRegister.Email, "Please, confirm Your registration", url);

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
        public async Task<IActionResult> Confirm([FromQuery] string encryptedEmail, [FromQuery] string encryptedPassword, [FromQuery] string firstName, [FromQuery] string lastName)
        {
            var user = await _userRepository.CreateAsync(new User { Email = encryptedEmail, Password = encryptedPassword, FirstName = firstName, LastName = lastName });

            if (user == null)
            {
                return Redirect($"{_configuration["ClientUrl"]}/confirm?success=false&message=User%20could%20not%20be%20created.");
            }

            var result = await LogIn(new UserLogInModel { Email = _cryptoService.Decrypt(user.Email), Password = _cryptoService.Decrypt(user.Password) });

            var okResult = result as OkObjectResult;

            if (okResult == null || okResult.Value == null || okResult.Value as LogInResponseModel == null)
            {
                return Redirect($"{_configuration["ClientUrl"]}/confirm?success=false&message=Failed%20during%20login%20user%20{_cryptoService.Decrypt(encryptedEmail)}");
            }

            return Redirect($"{_configuration["ClientUrl"]}/confirm?success=true&access_token={((LogInResponseModel)okResult.Value).access_token}&user_name={_cryptoService.Decrypt(encryptedEmail)}");
        }

        private async Task<ClaimsIdentity?> GetIdentity(UserLogInModel userLogIn)
        {
            var user = await _userRepository.GetAsync(_cryptoService.Encrypt(userLogIn.Email ?? throw new ArgumentException("Email is null")));

            if (user != null && user.Password == _cryptoService.Encrypt(userLogIn.Password))
            {
                var roles = await _roleRepository.GetListAsync(user.UserId);
                var roleType = string.Join(", ", roles.Select(x => x?.RoleName));

                var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email ?? "Anonymus"),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, roleType)
                    };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, roleType);

                return claimsIdentity;
            }

            return null;
        }

        private async Task<bool> DoesUserExist(string? email)
        {
            return await _userRepository.GetAsync(email) != null;
        }
    }
}

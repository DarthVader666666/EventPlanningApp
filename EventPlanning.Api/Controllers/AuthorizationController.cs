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

        public AuthorizationController(
            IRepository<User> userRepository, IRepository<Role> roleRepository, IMapper mapper, 
            IConfiguration configuration, CryptoService cryptoService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _configuration = configuration;
            _cryptoService = cryptoService;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> LogIn([FromBody] UserLogInModel userLogIn)
        {
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
                    expires: now.Add(TimeSpan.FromMinutes(10d)),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_configuration["SecurityKey"] ?? throw new ArgumentNullException("SecurityKey", "SecurityKey can't be null"))), SecurityAlgorithms.HmacSha256)
                    );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
             
            var response = new
            {
                access_token = encodedJwt,
                user_name = _cryptoService.Decrypt(identity.Name),
                role = identity.RoleClaimType
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("api/[controller]/register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel userRegister)
        {
            userRegister.Password = _cryptoService.Encrypt(userRegister.Password ?? throw new ArgumentException("Password is null"));
            userRegister.Email =  _cryptoService.Encrypt(userRegister.Email ?? throw new ArgumentException("Email is null"));

            if (await DoesUserExist(userRegister.Email))
            {
                return BadRequest(new { errorText = "User with this email already exists." });
            }

            var user = _mapper.Map<UserRegisterModel, User>(userRegister);
            user = await _userRepository.CreateAsync(user);

            if (user == null)
            {
                return BadRequest(new { errorText = "Error while creating user." });
            }

            return await LogIn(new UserLogInModel { Email = user.Email, Password = user.Password });
        }

        private async Task<ClaimsIdentity?> GetIdentity(UserLogInModel userLogIn)
        {
            var user = await _userRepository.GetAsync(_cryptoService.Encrypt(userLogIn.Email ?? throw new ArgumentException("Email is null")));

            if (user != null)
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

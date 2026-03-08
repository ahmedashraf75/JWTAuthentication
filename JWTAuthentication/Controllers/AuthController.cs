using JWTAuthentication.Configuration.Models;
using JWTAuthentication.Models;
using JWTAuthentication.Models.DTOs.Requests;
using JWTAuthentication.Models.DTOs.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
//please ask him how  he create a user table and in another side he use the identity user 
namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AuthController(UserManager<ApplicationUser> userManager,
                              IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterationRequestDto request)
        {
            if(ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new UserRegisterationResponseDto
                    {
                        Success = false,
                        Errors = new List<string> { "Email already in use" }
                    });
                }
                // creating a new user object with the provided email and password
                var newUser = new ApplicationUser
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    UserName = request.Email,
                    EmailConfirmed = true // todo i will make this feature later
                };
                //added the new user to the database
                var isCreated = await _userManager.CreateAsync(newUser, request.Password);

                if (!isCreated.Succeeded)//when the registration process fails,
                {
                    return BadRequest(new UserRegisterationResponseDto
                    {
                        Success = false,
                        // iscreated here is an IdentityResult object that contains
                        // the result of the user creation operation.
                        // If the operation failed, it will contain a list of errors
                        // that occurred during the process.
                        // The code is selecting the description of each error
                        // and returning it as a list of strings in the response.
                        Errors = isCreated.Errors.Select(x => x.Description).ToList()
                    });
                }
                // if the registration process is successful, the code generates a JWT token for the newly created user
                var jwtToken = GenerateJwtToken(newUser);
                return Ok(new UserRegisterationResponseDto
                {
                    Success = true,
                    Token = jwtToken
                });
            }
            else
            {
                return BadRequest(new UserRegisterationResponseDto
                {
                    Success = false,
                    Errors = new List<string> { "Invalid payload" }
                });
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto requestDto)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(requestDto.Email);
                if (existingUser == null)
                {
                    return BadRequest(new UserLoginResponseDto
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid login request" }
                    });
                }
                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, requestDto.Password);
                if (!isCorrect)
                {
                    return BadRequest(new UserLoginResponseDto
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid login request" }
                    });
                }
                var jwtToken = GenerateJwtToken(existingUser);
                return Ok(new UserLoginResponseDto
                {
                    Success = true,
                    Token = jwtToken
                });
            }
            else
            {
                return BadRequest(new UserLoginResponseDto
                {
                    Success = false,
                    Errors = new List<string> { "Invalid payload" }
                });
            }
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            // this code is creating a new instance of the JwtSecurityTokenHandler class,
            // which is responsible for creating and validating JWT tokens.
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            // this code is converting the secret key from the JWT configuration into a byte array using UTF-8 encoding.
            var key = System.Text.Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //the subject is a collection of claims that will be included in the JWT token.
                //Claims are pieces of information about the user that can be used by the application
                //to identify and authorize the user.
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpireTimeFrame),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}

using HotelListingAPI.Models.User;
using HotelListingAPI.Services.AuthManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HotelListingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthManager authManager, ILogger<AuthController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        [HttpPost("registration")]
        // Document possible response types
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [FromBody] --> Makes sure the info comes from the body
        public async Task<IActionResult> Registration([FromBody] ApiUserDto apiUserDto)
        {
            _logger.LogInformation($"{apiUserDto.Email} is trying to registered: {DateTime.Now.Date}");

            try
            {
                var errors = await _authManager.RegisterUser(apiUserDto);
                var identityErrors = errors as IdentityError[] ?? errors.ToArray();
            
                if (!identityErrors.Any()) return Ok("User has successfully been added");
            
                foreach (var error in identityErrors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"Something went wrong in the {nameof(Registration)} - User Registration for: {apiUserDto.Email}: {DateTime.Now.Date}");
                return Problem($"Something went wrong in the {nameof(Registration)}", "Contact support",
                    statusCode: 500);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            _logger.LogInformation($"{loginDto.Email} is trying to log in: {DateTime.Now.Date}");

            try
            {
                var authResponse = await  _authManager.Login(loginDto);

                if (authResponse.Token.IsNullOrEmpty() || authResponse.UserId.IsNullOrEmpty())
                {
                    return Unauthorized();
                }
            
                return Ok(authResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"Something went wrong in the {nameof(Login)} - User Login for: {loginDto.Email}: {DateTime.Now.Date}");
                return Problem($"Something went wrong in the {nameof(Login)}", "Contact support",
                    statusCode: 500);
            }
        }
        
        [HttpPost("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromBody]AuthResponse request)
        {
            var authResponse = await  _authManager.VerifyRefreshAuthToken(request);

            if (authResponse is null)
            {
                return Unauthorized();
            }
            
            return Ok(authResponse);
        }
    }
}

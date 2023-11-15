using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListingAPI.Models.User;
using HotelListingAPI.Services.AuthManager;
using Microsoft.AspNetCore.Http;
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

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost("registration")]
        // Document possible response types
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [FromBody] --> Makes sure the info comes from the body
        public async Task<IActionResult> Registration([FromBody] ApiUserDto apiUserDto)
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

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            var authResponse = await  _authManager.Login(loginDto);

            if (authResponse.Token.IsNullOrEmpty() || authResponse.UserId.IsNullOrEmpty())
            {
                return Unauthorized();
            }
            
            return Ok(authResponse);
        }
    }
}

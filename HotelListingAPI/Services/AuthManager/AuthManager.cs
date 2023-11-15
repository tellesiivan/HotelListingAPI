using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HotelListingAPI.Services.AuthManager;

public class AuthManager: IAuthManager
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;
    
    public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
    {
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
    }
    
    public async Task<IEnumerable<IdentityError>> RegisterUser(ApiUserDto userDto)
    {
        if (!userDto.Password.Equals(userDto.PasswordConfirm))
        {
            return new List<IdentityError>()
            {
                new IdentityError()
                {
                    Description = "Passwords do not match"
                }
            };
        }
        
        var user = _mapper.Map<ApiUser>(userDto);
        // manually set username to the passed email
        user.UserName = userDto.Email;

        // hash the password and password salt
        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
        }
        
        // directly return the errors since it can possibly have errors if it did not succeeded
        // or if it did there will be an empty list 
        return result.Errors;
    }

    public async Task<AuthResponse> Login(LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (user is null)
            {
                throw new Exception("User was not found");
            }
            
            var validPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            
            if (!validPassword)
            {
                throw new Exception("Password is not valid");
            }

            var token = await GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                UserId = user.Id
            };

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private async Task<string> GenerateToken(ApiUser user)
    {
        var secretKey = (string)_configuration["JwtSettings:Key"]!;
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials =
            new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);

        // create a list of claims with the claim type of Role
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(user);

        var claims = new List<Claim>
            {
                // subject who the token has been issued to
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? user.FirstName),
                //
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? user.FirstName),
                // you  an add a custom claim type
                new Claim("uid", user.Id)
            }
            .Union(roleClaims).Union(userClaims);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience:_configuration["JwtSettings:Audience"],
            claims,
            expires: DateTime.UtcNow.AddDays((1)),
            signingCredentials: credentials
            );
        
        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}
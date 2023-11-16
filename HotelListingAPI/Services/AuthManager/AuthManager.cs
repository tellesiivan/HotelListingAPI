using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using static System.String;

namespace HotelListingAPI.Services.AuthManager;

public class AuthManager: IAuthManager
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;
    private ApiUser _apiUser;
    private const string _tokenProvider = "HotelListingAPI";
    private const string _tokenName = "RefreshToken";

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
        _apiUser = _mapper.Map<ApiUser>(userDto);
        // manually set username to the passed email
        _apiUser.UserName = userDto.Email;

        // hash the password and password salt
        var result = await _userManager.CreateAsync(_apiUser, userDto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(_apiUser, "User");
        }
        
        // directly return the errors since it can possibly have errors if it did not succeeded
        // or if it did there will be an empty list 
        return result.Errors;
    }

    public async Task<AuthResponse> Login(LoginDto loginDto)
    {
        try
        {
            
            var foundUser = await _userManager.FindByEmailAsync(loginDto.Email);
            _apiUser = foundUser ?? throw new Exception("User was not found");
            
            var validPassword = await _userManager.CheckPasswordAsync(_apiUser, loginDto.Password);
            
            if (!validPassword)
            {
                throw new Exception("Password is not valid");
            }

            var token = await GenerateToken();

            return new AuthResponse
            {
                Token = token,
                UserId = _apiUser.Id,
                RefreshToken = await CreateRefreshToken()
            };

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<string> CreateRefreshToken()
    {
        // remove the old token
        await _userManager.RemoveAuthenticationTokenAsync(_apiUser, _tokenProvider, _tokenName);
        // create new token
        var refreshedTokenCreated =
            await _userManager.GenerateUserTokenAsync(_apiUser, _tokenProvider,
                _tokenName);
        // set new token
        var tokenResponse = await _userManager.SetAuthenticationTokenAsync(_apiUser,
            _tokenProvider,
            _tokenName, refreshedTokenCreated);
            
        return !tokenResponse.Succeeded ? "Unable to create new token" : refreshedTokenCreated;
    }

    public async Task<AuthResponse?> VerifyRefreshAuthToken(AuthResponse request)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
        var userEmail = tokenContent.Claims.ToList()
            .FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email)?.Value;

        if (userEmail is null) return null;
        
        var userFound = await _userManager.FindByEmailAsync(userEmail);

        if (userFound is null ) return null;
        var userInfoMatches = string
            .Equals(userFound.Id, request.UserId, StringComparison.CurrentCultureIgnoreCase);
        
        if (!userInfoMatches) return null;
        
        _apiUser = userFound;

        var isValidToken =
            await _userManager.VerifyUserTokenAsync(_apiUser, _tokenProvider, _tokenName,
                request.Token);

        if (!isValidToken)
        {
             await _userManager.UpdateSecurityStampAsync(_apiUser);
             return null;
        };

        var token = await GenerateToken();
        return new AuthResponse
        {
            Token = token, UserId = _apiUser.Id, RefreshToken = await CreateRefreshToken()
        };
    }

    private async Task<string> GenerateToken()
    {
        var secretKey = (string)_configuration["JwtSettings:Key"]!;
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials =
            new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(_apiUser);

        // create a list of claims with the claim type of Role
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(_apiUser);

        var claims = new List<Claim>
            {
                // subject who the token has been issued to
                new Claim(JwtRegisteredClaimNames.Sub, _apiUser.Email ?? _apiUser.FirstName),
                //
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _apiUser.Email ?? _apiUser.FirstName),
                // you  an add a custom claim type
                new Claim("uid", _apiUser.Id)
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
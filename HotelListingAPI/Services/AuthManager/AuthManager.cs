using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListingAPI.Services.AuthManager;

public class AuthManager: IAuthManager
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    
    public AuthManager(IMapper mapper, UserManager<ApiUser> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
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

    public async Task<bool> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        
        if (user is null)
        {
            return false;
        }
        
        var validPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        return validPassword;
    }
}
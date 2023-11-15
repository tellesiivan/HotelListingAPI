using HotelListingAPI.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListingAPI.Services.AuthManager;

public interface IAuthManager
{
    Task<IEnumerable<IdentityError>> RegisterUser(ApiUserDto userDto);
    Task<AuthResponse> Login(LoginDto loginDto);
    
}

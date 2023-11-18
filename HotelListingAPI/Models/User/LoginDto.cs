using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.Models.User;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(15, ErrorMessage = "Your password is limited to {2} to {1} characters", MinimumLength = 6)]
    public string Password { get; set; } = "";
}
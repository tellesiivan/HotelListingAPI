using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.Models.User;

public class ApiUserDto: LoginDto
{
    [Required]
    public string FirstName { get; set; } =  string.Empty;
    [Required]
    public string LastName { get; set; } =  string.Empty;
    [Required]
    [Compare("Password")]
    public string PasswordConfirm { get; set; } = "";

}
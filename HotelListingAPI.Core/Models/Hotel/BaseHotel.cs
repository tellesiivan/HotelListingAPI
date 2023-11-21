using HotelListingAPI.Data;
using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.Models.Hotel;

public class BaseHotel
{
    [Required]
    public string Name { get; set; } = "";
    [Required]
    public string Address { get; set; } = "";
    [Required]
    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }
    public Rating? Rating { get; set; }
}
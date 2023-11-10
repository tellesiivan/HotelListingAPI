using HotelListingAPI.Data;

namespace HotelListingAPI.Models.Hotel;

public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public Rating Rating { get; set; } = Rating.FiveStar;
    public int CountryId { get; set; }
}
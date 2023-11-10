using HotelListingAPI.Models.Hotel;

namespace HotelListingAPI.Models.Country;

public class CountryDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string ShortName { get; set; } = "";
    public List<HotelDto>? Hotels { get; set; }

}
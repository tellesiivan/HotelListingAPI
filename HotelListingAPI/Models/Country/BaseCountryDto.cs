using Microsoft.Build.Framework;

namespace HotelListingAPI.Models.Country;

// abstract -> can't initialize them as stand alone classes but can be inherited
public abstract class BaseCountryDto
{
    [Required]
    public string Name { get; set; } = "";
    public string ShortName { get; set; } = "";
}
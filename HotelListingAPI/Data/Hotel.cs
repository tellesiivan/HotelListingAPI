using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListingAPI.Data;

public class Hotel
{
    // primary key
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public Rating Rating { get; set; } = Rating.FiveStar;
    // [ForeignKey("CountryId")] : Can manually add the name OR reference it by field name
    [ForeignKey(nameof(CountryId))]
    public int CountryId { get; set; }
    public Country Country { get; set; } 
}
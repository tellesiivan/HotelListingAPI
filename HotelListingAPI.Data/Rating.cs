using System.Text.Json.Serialization;

namespace HotelListingAPI.Data;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Rating
{
    OneStar = 1,
    TwoStar,
    ThreeStar,
    FourStar,
    FiveStar
}
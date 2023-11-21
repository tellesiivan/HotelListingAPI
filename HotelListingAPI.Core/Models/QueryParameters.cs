namespace HotelListingAPI.Models;

public class QueryParameters
{
    public int StartIndex { get; set; }

    public int PageSize { get; set; } = 15;

    public int PageNumber { get; set; }

}

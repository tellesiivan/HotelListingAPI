namespace HotelListingAPI.Models;


public class QueryResult<T>
{
    // how many records are there in all
    public int TotalCount { get; set; }
    // page number
    public int PageNumber { get; set; }
    // record number(how many items are coming back)
    public int RecordNumber { get; set; }
    // List of the data type they requested
    public List<T> Data { get; set; }
}
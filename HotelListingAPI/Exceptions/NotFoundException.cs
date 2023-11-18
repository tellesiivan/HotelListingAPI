namespace HotelListingAPI.Exceptions;

public class NotFoundException: ApplicationException
{
    // base(name) is refers to ApplicationException 
    public NotFoundException(string name, object key): base($"{name} ({key}) was not found")
    {
        
    }
}
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.Data;

public class HotelListingDbContext:DbContext
{
    // getting the options from AddDbContext in Program.cs and passes it down to the DbContext inheritance
    public HotelListingDbContext(DbContextOptions contextOptions): base(contextOptions)
    {
    }
    
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }
}
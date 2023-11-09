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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // When the model is being build, this is what should happen.
        // Seed Country model based data
        modelBuilder.Entity<Country>().HasData(
            new Country() { Id = 1, ShortName = "USA", Name = "United States" },
            new Country() { Id = 2, ShortName = "MEX", Name = "Mexico" },
            new Country() { Id = 3, ShortName = "ENG", Name = "England" },
            new Country() { Id = 4, ShortName = "FR", Name = "France" }
        );

        modelBuilder.Entity<Hotel>().HasData(
            new Hotel()
            {
                Id = 1,
                CountryId = 1,
                Address = "4302 East street",
                Name = "Fairfield",
                Rating = Rating.FourStar
            },
        new Hotel()
            {
                Id = 2,
                CountryId = 3,
                Address = "123 Valencia street",
                Name = "Western",
                Rating = Rating.FiveStar
            },
        new Hotel()
        {
            Id = 3, CountryId = 4, Address = "8232 Bread street", Name = "Paris", Rating = Rating.OneStar
        }
        );
    }
}
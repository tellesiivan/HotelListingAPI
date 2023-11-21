using HotelListingAPI.Data.Configs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.Data;

public class HotelListingDbContext:IdentityDbContext<ApiUser>
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

        // Seed model based data
        // When the model is being build, this is what should happen.
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new CountryConfiguration());
        modelBuilder.ApplyConfiguration(new HotelConfiguration());
    }
}
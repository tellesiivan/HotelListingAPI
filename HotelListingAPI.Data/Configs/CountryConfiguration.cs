using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingAPI.Data.Configs;

public class CountryConfiguration: IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasData(
            new Country() { Id = 1, ShortName = "USA", Name = "United States" },
            new Country() { Id = 2, ShortName = "MEX", Name = "Mexico" },
            new Country() { Id = 3, ShortName = "ENG", Name = "England" },
            new Country() { Id = 4, ShortName = "FR", Name = "France" }
        );
    }
}
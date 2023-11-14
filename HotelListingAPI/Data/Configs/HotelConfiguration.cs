using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingAPI.Data.Configs;

public class HotelConfiguration: IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasData(
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
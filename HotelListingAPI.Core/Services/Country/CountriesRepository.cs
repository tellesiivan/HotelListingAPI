using AutoMapper;
using HotelListingAPI.Data;
using Microsoft.EntityFrameworkCore;
using CountryDataModel = HotelListingAPI.Data.Country;

namespace HotelListingAPI.Services.Country;

public class CountriesRepository: GenericRepository<CountryDataModel>, ICountriesRepository
{
    private readonly HotelListingDbContext _dbContext;
    
    public CountriesRepository(HotelListingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
        _dbContext = dbContext;
    }

    public async Task<CountryDataModel?> GetCountryDetails(int id)
    {

        var country = await _dbContext.Countries
            .Include((country) => country.Hotels)
            .FirstOrDefaultAsync((country) => country.Id == id);


        return country;
    }
}
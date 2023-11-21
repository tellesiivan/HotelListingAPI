using CountryDataModel = HotelListingAPI.Data.Country;

namespace HotelListingAPI.Services.Country;

public interface ICountriesRepository:IGenericRepository<CountryDataModel>
{
   Task<CountryDataModel?> GetCountryDetails(int id);
}
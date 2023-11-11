using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.Models.Country;
using HotelListingAPI.Models.Hotel;

namespace HotelListingAPI.Configurations;

public class AutoMapperConfig: Profile
{
    public AutoMapperConfig()
    {
        // Reverse in either direction by adding .ReverseMap
        // <MapFrom, MapTo>
        CreateMap<Country, CountryDto>().ReverseMap();
        CreateMap<Country, GetCountryDto>().ReverseMap();
        CreateMap<Country, CountryDetailsDto>().ReverseMap();
        CreateMap<Country, UpdateCountryDto>().ReverseMap();
        
        CreateMap<Hotel, HotelDto>().ReverseMap();
        CreateMap<Hotel, BaseHotel>().ReverseMap();
    }
}
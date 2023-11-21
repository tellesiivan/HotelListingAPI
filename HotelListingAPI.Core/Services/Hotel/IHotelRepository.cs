using HotelDataModel = HotelListingAPI.Data.Hotel;
namespace HotelListingAPI.Services.Hotel;

public interface IHotelRepository: IGenericRepository<HotelDataModel>
{
}
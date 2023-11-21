using AutoMapper;
using HotelListingAPI.Data;
using HotelDataModel = HotelListingAPI.Data.Hotel;
namespace HotelListingAPI.Services.Hotel;

public class HotelRepository: GenericRepository<HotelDataModel>, IHotelRepository
{
    private readonly HotelListingDbContext _dbContext;

    public HotelRepository(HotelListingDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
        _dbContext = dbContext;
    }
}
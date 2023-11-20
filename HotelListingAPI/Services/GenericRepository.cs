using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListingAPI.Data;
using HotelListingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.Services;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{

    private readonly HotelListingDbContext _context;
    private readonly IMapper _mapper;
    
    protected GenericRepository(HotelListingDbContext dbContext, IMapper mapper)
    {
        _mapper = mapper;
        _context = dbContext;
    }
    
    public async Task<T?> GetAsync(int? id)
    {
        if (id is null)
        {
            return null;
        }

        var entityResponse = await _context.Set<T>().FindAsync(id);

        return entityResponse ?? null;
    }

    public async Task<List<TResult>> GetAllAsync<TResult>()
    {
        // example: DbSet<Country> (Get the data with the associated model<T>)
        return await _context.Set<T>().
            ProjectTo<TResult>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<QueryResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters)
    {
        var totalSize = await _context.Set<T>().CountAsync();
        var items = await _context.Set<T>()
            .Skip(queryParameters.StartIndex)
            .Take(queryParameters.PageSize)
            // exact columns it should query
            .ProjectTo<TResult>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new QueryResult<TResult>()
        {
            Data = items,
            RecordNumber = items.Count(),
            TotalCount = totalSize,
            PageNumber = queryParameters.PageNumber
        };

    }

    public async Task<T> AddAsync(T entity)
    {
        try
        {
            // EF will detect what type entity<T> is and update it correspondingly
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetAsync(id);

        if (entity is not null)
        {
            _context.Remove<T>(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(T entity)
    {
         _context.Update(entity);
         await _context.SaveChangesAsync();
    }

    public async Task<bool> DoesExists(int id)
    {
        var entity = await GetAsync(id);
        return entity is not null;
    }
}
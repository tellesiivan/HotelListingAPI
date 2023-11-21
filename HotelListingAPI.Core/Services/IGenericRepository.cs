using HotelListingAPI.Models;

namespace HotelListingAPI.Services;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetAsync(int? id);
    Task<List<TResult>> GetAllAsync<TResult>();
    Task<QueryResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
    Task<T> AddAsync(T entity);
    Task DeleteAsync(int id);
    Task UpdateAsync(T entity);

    Task<bool> DoesExists(int id);
}
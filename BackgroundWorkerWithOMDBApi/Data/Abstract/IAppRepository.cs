using BackgroundWorkerWithOMDBApi.Entities;

namespace BackgroundWorkerWithOMDBApi.Data.Abstract;
public interface IAppRepository
{
    // methods which will be implemented in other classes :
    Task<List<Movie>> GetAll();
    Task AddAsync<T>(T entity) where T : class;
    Task DeleteAsync<T>(T entity) where T : class;
    Task UpdateAsync<T>(T entity) where T : class;
    Task<bool> SaveAllAsync();
}

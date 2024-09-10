using BackgroundWorkerWithOMDBApi.Data.Abstract;
using BackgroundWorkerWithOMDBApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackgroundWorkerWithOMDBApi.Data.Concrete
{
    public class AppRepository : IAppRepository
    {
        // Private field for the DbContext
        private readonly OMDBMovieDBContext _context;

        // Constructor for dependency injection
        public AppRepository(OMDBMovieDBContext context)
        {
            _context = context;
        }

        // Add a new entity to the context
        public async Task AddAsync<T>(T entity) where T : class
        {
            _context.Add(entity);
            await SaveAllAsync(); // Optionally save changes immediately
        }

        // Delete an entity from the context
        public async Task DeleteAsync<T>(T entity) where T : class
        {
            _context.Remove(entity);
            await SaveAllAsync(); // Optionally save changes immediately
        }

        // Update an existing entity in the context
        public async Task UpdateAsync<T>(T entity) where T : class
        {
            _context.Update(entity);
            await SaveAllAsync(); // Optionally save changes immediately
        }

        // Save all changes to the database
        public async Task<bool> SaveAllAsync()
        {
            try
            {
                return (await _context.SaveChangesAsync()) > 0;
            }
            catch (DbUpdateException ex)
            {
                // Log exception details here (using a logger or any other logging mechanism)
                throw new Exception("An error occurred while saving changes.", ex);
            }
        }

        // Get all movies from the database
        public async Task<List<Movie>> GetAll()
        {
            return await _context.Movies.ToListAsync();
        }
    }
}

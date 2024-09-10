using BackgroundWorkerWithOMDBApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackgroundWorkerWithOMDBApi.Data;
public class OMDBMovieDBContext : DbContext
{
    // parametric constructor for injecting : 
    public OMDBMovieDBContext(DbContextOptions<OMDBMovieDBContext> opt) : base(opt) { }

    // tables for DB : 
   public DbSet<Movie> Movies { get; set; }
}

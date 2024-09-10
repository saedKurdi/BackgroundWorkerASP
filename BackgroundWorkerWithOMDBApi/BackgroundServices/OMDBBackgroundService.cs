using BackgroundWorkerWithOMDBApi.Data.Abstract;
using BackgroundWorkerWithOMDBApi.Entities;
using BackgroundWorkerWithOMDBApi.OtherClasses;
using System.Text.Json;

namespace BackgroundWorkerWithOMDBApi.BackgroundServices;
public class OMDBBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OMDBBackgroundService> _logger;
    private readonly HttpClient _httpClient;
    private TimeSpan _interval;
    private static readonly Random RandomGenerator = new Random();

    public OMDBBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<OMDBBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;

        int intervalMinutes = int.TryParse(_configuration["Settings:TimeInterval"], out int result) ? result : 10;
        _interval = TimeSpan.FromSeconds(intervalMinutes);

        // Initialize HttpClient
        _httpClient = new HttpClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var appRepository = scope.ServiceProvider.GetRequiredService<IAppRepository>();
                await SendRequestAndAddMovieToDB(appRepository);
                await FetchDataFromDatabaseAsync(appRepository);
            }
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task FetchDataFromDatabaseAsync(IAppRepository appRepository)
    {
        try
        {
            var movies = await appRepository.GetAll();
            _logger.LogInformation($"Fetched {movies.Count} movies from database.");
            if (movies.Count == 0)
            {
                _logger.LogWarning("No movies found in the database.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching data from database: {ex.Message}");
        }
    }


    private async Task SendRequestAndAddMovieToDB(IAppRepository appRepository)
    {
        // Generate a random letter
        char randomLetter1 = GenerateRandomLetter();
        char randomLetter2 = GenerateRandomLetter();
        char randomLetter3 = GenerateRandomLetter();

        // OMDB API request
        string apiKey = _configuration["OMDB:ApiKey"];
        string url = $"http://www.omdbapi.com/?s={String.Concat(randomLetter1,randomLetter2,randomLetter3)}&apikey={apiKey}";

        try
        {
            // Fetch raw JSON response
            // await _httpClient.GetFromJsonAsync<OMDBResponse>(url);
            var responseMessage = await _httpClient.GetAsync(url);
            var rawJson = await responseMessage.Content.ReadAsStringAsync();

            // Deserialize the JSON response
            var response = JsonSerializer.Deserialize<OMDBResponse>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (response != null && response.Search != null)
            {
                foreach (var movie in response.Search)
                {
                    var existingMovies = await appRepository.GetAll();
                    if (!existingMovies.Any(m => m.Title == movie.Title))
                    {
                        // Add the movie to the database
                        var newMovie = new Movie
                        {
                            Title = movie.Title,
                            Type = movie.Type,
                            ImdbID = movie.ImdbID,
                            Poster = movie.Poster,
                            Year = movie.Year
                        };

                        await appRepository.AddAsync(newMovie);
                        await appRepository.SaveAllAsync();
                        _logger.LogInformation($"Added movie: {newMovie.Title}");
                        break;
                    }
                    continue;
                }
            }
            else
            {
                _logger.LogWarning("No movies found or response.Search is null.");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Error fetching data from OMDB API: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error: {ex.Message}");
        }
    }


    private char GenerateRandomLetter()
    {
        return (char)RandomGenerator.Next('a', 'z' + 1);
    }
}

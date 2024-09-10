﻿namespace BackgroundWorkerWithOMDBApi.Entities;
public class Movie
{
    // public properties : 
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Year { get; set; }
    public string? ImdbID { get; set; }
    public string? Type { get; set; }
    public string? Poster { get; set; }
}

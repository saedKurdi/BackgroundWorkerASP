using BackgroundWorkerWithOMDBApi.BackgroundServices;
using BackgroundWorkerWithOMDBApi.Data;
using BackgroundWorkerWithOMDBApi.Data.Abstract;
using BackgroundWorkerWithOMDBApi.Data.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// adding injections of repoisotries : 
builder.Services.AddScoped<IAppRepository, AppRepository>();

// adding db context : 
var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<OMDBMovieDBContext>(opt =>
{
    opt.UseSqlServer(conn);
});

// registering background service : 
builder.Services.AddHostedService<OMDBBackgroundService>();

// creating building app :
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

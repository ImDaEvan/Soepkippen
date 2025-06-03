using Microsoft.EntityFrameworkCore;
using SoepkipAPI.Data.Context;
using SoepkipAPI.Data.Interfaces;
using SoepkipAPI.Data.Repository;
using SoepkipAPI.Controllers;
using SoepkipAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<WeatherService>();

//Depencency injection
builder.Services.AddTransient<ITrashRepository, TrashRepository>();

//Add database connection
builder.Services.AddDbContext<TrashContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(Environment.GetEnvironmentVariable("CONNECTION_STRING_MYSQL") ?? "DefaultConnection"));
});

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
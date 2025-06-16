namespace SoepkipAPI.Data.Interfaces;

public interface IWeatherService
{
    Task<WeatherData?> GetWeatherAsync(string location);
}
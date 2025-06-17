namespace SoepkipAPI.Data.Interfaces;

public interface IWeatherService
{
    Task<WeatherData?> GetWeatherAsync(float longditude, float latitude);
}
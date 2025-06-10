using Microsoft.AspNetCore.Mvc;
using SoepkipAPI.Services;

namespace SoepkipAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherTestController : ControllerBase
{
    private readonly WeatherService _weatherService;

    public WeatherTestController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    // GET: api/weathertest
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var weather = await _weatherService.GetWeatherAsync("Breda");

            if (weather == null)
                return StatusCode(503, "Geen weerdata ontvangen van de API.");

            return Ok(new
            {
                Temperature = weather.temp,
                FeelsLike = weather.gtemp,
                WindSpeedKmh = weather.windkmh,
                WindDirection = weather.windr,
                Date = weather.datum,
                Time = weather.tijd,
                Timestamp = weather.Timestamp
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Fout bij ophalen van weerdata: {ex.Message}");
        }
    }
}

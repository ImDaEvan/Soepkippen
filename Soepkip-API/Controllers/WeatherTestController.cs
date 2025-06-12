using Microsoft.AspNetCore.Mvc;
using SoepkipAPI.Models;
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

            return Ok(new TrashItem()
            {
                timestamp = weather.DateTime,
                feels_like_temp_celsius = weather.gtemp,
                actual_temp_celsius = weather.temp,
                wind_force_bft = weather.windms, //TODO: convert ms to bft
                wind_direction = weather.windrgr,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Fout bij ophalen van weerdata: {ex.Message}");
        }
    }
}

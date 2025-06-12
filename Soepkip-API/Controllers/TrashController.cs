using Microsoft.AspNetCore.Mvc;
using SoepkipAPI.Data.Interfaces;
using SoepkipAPI.Models;
using SoepkipAPI.Services;

namespace SoepkipAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TrashController : Controller
{
    private readonly ITrashRepository _trashRepository;
    private readonly ILogger<TrashController> _logger;
    private readonly WeatherService _weatherService;

    public TrashController(ITrashRepository trashRepository, ILogger<TrashController> logger, WeatherService weatherService)
    {
        _trashRepository = trashRepository;
        _logger = logger;
        _weatherService = weatherService;
    }

    // GET: api/trash?dateLeft=a&dateRight=b
    [HttpGet]
    public IActionResult ReadRange(string dateLeft, string dateRight)
    {
        try
        {
            if (!DateTime.TryParse(dateLeft, out var dateLeftParsed))
                throw new("Left date couldn't be parsed");

            if (!DateTime.TryParse(dateRight, out var dateRightParsed))
                throw new("Right date couldn't be parsed");

            var trash = _trashRepository.ReadRange(dateLeftParsed, dateRightParsed);
            return Ok(trash);
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest();
        }
    }

    // POST: api/trash
    [HttpPost]
    public async Task<IActionResult> Write([FromBody] TrashItem trash)
    {
        try
        {
            _trashRepository.Write(trash);

            // Enrich trash data with weather info
            var weather = await _weatherService.GetWeatherAsync("Breda");
            if (weather != null)
            {
                trash.actual_temp_celsius = weather.temp;
                trash.feels_like_temp_celsius = weather.gtemp;
                trash.wind_force_bft = weather.windms;
                trash.wind_direction = weather.windrgr;
            }

            var rowsAffected = await _trashRepository.SaveChangesAsync();
            if (rowsAffected == 0)
                throw new("Writing trash to the context resulted in nothing happening");

            return Ok(rowsAffected);
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest();
        }
    }
}

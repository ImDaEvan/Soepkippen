using Microsoft.AspNetCore.Authorization;
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
    private readonly IWeatherService _weatherService;

    public TrashController(ITrashRepository trashRepository, ILogger<TrashController> logger, IWeatherService weatherService)
    {
        _trashRepository = trashRepository;
        _logger = logger;
        _weatherService = weatherService;
    }

    // GET: api/trash?dateLeft=a&dateRight=b
    /// <summary>
    /// Retrieves all trash detections between <paramref name="dateLeft"/> and <paramref name="dateRight"/>.
    /// </summary>
    /// <param name="dateLeft"></param>
    /// <param name="dateRight"></param>
    /// <returns></returns>
    /// <response code = "400">Either the date was in an invalid format or dateLeft > dateRight.</response>
    /// <response code = "401">Unauthorized access.</response>
    /// <response code = "404">No trash detections found in the specified range.</response>

    [HttpGet]
    [Authorize(AuthenticationSchemes = "monitoring")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public IActionResult GetTrash([FromQuery] string dateLeft, [FromQuery] string dateRight)
    {
        try
        {
            if (!_trashRepository.TryParseIsoUtc(dateLeft, out var from)) // Fixed by removing the incorrect static call
            {
                return BadRequest("Left date couldn't be parsed");
            }
            if (!_trashRepository.TryParseIsoUtc(dateRight, out var to)) // Fixed by removing the incorrect static call
            {
                return BadRequest("dateRight is in an invalid format!");
            }
            if (from > to)
            {
                return BadRequest("dateLeft must be earlier than dateRight!");
            }

            var detections = _trashRepository.ReadRange(from, to);

            return Ok(detections);
        }
        catch
        {
            _logger.LogError("An error occurred while retrieving trash detections.");
            return BadRequest("An error occurred while processing your request.");
        }
    }

    // POST: api/trash
    [Authorize(AuthenticationSchemes = "sensoring")]
    [HttpPost]
    public async Task<IActionResult> Write([FromBody] List<TrashItem> trashItems)
    {
        try
        {
            foreach (var trashItem in trashItems)
            {
                //Only enrich data if there's a location
                if (trashItem.longditude != null && trashItem.latitude != null)
                {
                    // Enrich trash data with weather info
                    var weather = await _weatherService.GetWeatherAsync((float)trashItem.longditude, (float)trashItem.latitude);

                    if (weather != null)
                    {
                        trashItem.actual_temp_celsius = weather.Temp;
                        trashItem.feels_like_temp_celsius = weather.GTemp;
                        trashItem.wind_force_bft = weather.WindBft;
                        trashItem.wind_direction = weather.WindrGr;
                        trashItem.weather_timestamp = weather.ParsedTime;
                    }
                }

                //test change
                _trashRepository.Write(trashItem);


            }
            var rowsAffected = await _trashRepository.SaveChangesAsync();
            if (rowsAffected == 0)
                throw new("Writing trash to the context resulted in nothing happening");
            return Ok(rowsAffected);
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest($"Something went wrong: {e.Message}");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SoepkipAPI.Data.Interfaces;
using SoepkipAPI.Models;
using SoepkipAPI.Services;
using System.Text.Json;

namespace SoepkipAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VersionController : Controller
{
    private readonly ILogger<VersionController> _logger;

    public VersionController( ILogger<VersionController> logger)
    {
        _logger = logger;
    }

    private Dictionary<string, JsonElement> GetJsonFile()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "version.json");

        
        var json = System.IO.File.ReadAllText(filePath);

        // Deserialize as Dictionary<string, JsonElement> to support both objects and strings (like "current": "4")
        var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        return result;
    }

    [HttpGet("full")]
    public IActionResult GetVersionData()
    {
        try
        {
            var result = GetJsonFile();
            if (result == null)
            {
                _logger.LogError("Deserialization of version.json returned null.");
                return StatusCode(500, "Failed to parse version.json.");
            }
            else
            {
                _logger.LogInformation("Version data loaded: " + JsonSerializer.Serialize(result));
            }
                return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest();
        }
    }

    [HttpGet("latest")]
    public IActionResult GetLatestVersionData()
    {
        try
        {
            var result = GetJsonFile();
            
            Dictionary<string,JsonElement> dict = new Dictionary

            if (result == null)
            {
                _logger.LogError("Deserialization of version.json returned null.");
                return StatusCode(500, "Failed to parse version.json.");
            }
            else
            {
                _logger.LogInformation("Version data loaded: " + JsonSerializer.Serialize(result));
            }
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest();
        }
    }
}
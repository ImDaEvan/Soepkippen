using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SoepkipAPI.Data.Interfaces;
using SoepkipAPI.Models;
using SoepkipAPI.Services;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace SoepkipAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChangelogController : Controller
{
    private readonly ILogger<ChangelogController> _logger;

    public ChangelogController( ILogger<ChangelogController> logger)
    {
        _logger = logger;
    }

    private Dictionary<string, JsonElement> GetJsonFile()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "version.json");

        
        var json = System.IO.File.ReadAllText(filePath);

        // Deserialize as Dictionary<string, JsonElement> to support both objects and strings (like "current": "4")
        var result = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        return result;
    }

    [HttpGet("all")]
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
                _logger.LogInformation("Version data loaded: " + System.Text.Json.JsonSerializer.Serialize(result));
            }
                return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest();
        }
    }
    [HttpGet]
    public IActionResult GetChangelogByQuery([FromQuery] string version)
    {
        try
        {
            var result = GetJsonFile(); // assuming this loads your version.json as Dictionary<string, JsonElement>

            if (result == null)
            {
                _logger.LogError("Deserialization of version.json returned null.");
                return StatusCode(500, "Failed to parse version.json.");
            }

            if (!result.ContainsKey(version))
            {
                return NotFound($"Version {version} not found.");
            }

            var versionData = result[version];
            var response = new
            {
                version = version,
                changelog = versionData
            };
            return Ok(response);
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

            if (result == null)
            {
                _logger.LogError("Deserialization of version.json returned null.");
                return StatusCode(500, "Failed to parse version.json.");
            }

            string currentVersion = result["current"].GetString();

            if (!result.ContainsKey(currentVersion))
            {
                return NotFound($"Version {currentVersion} not found.");
            }

            string currentChangelog = result[currentVersion].GetString();

            var response = new
            {
                version = currentVersion,
                changelog = currentChangelog
            };

            _logger.LogInformation("Version data loaded: " + System.Text.Json.JsonSerializer.Serialize(response));
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest();
        }
    }

}
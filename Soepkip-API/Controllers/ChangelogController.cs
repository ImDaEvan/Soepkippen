using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoepkipAPI.Data.Interfaces;
using SoepkipAPI.Models;
using SoepkipAPI.Services;
using System.Runtime.CompilerServices;

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

    private Dictionary<string, string> GetJsonFile()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "version.json");
        
        var json =  System.IO.File.ReadAllText(filePath);

        // Deserialize as Dictionary<string, JsonElement> to support both objects and strings (like "current": "4")
        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        return result;
    }

    [HttpGet]
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
                _logger.LogInformation("Version data loaded: " + JsonConvert.SerializeObject(result));
            }
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest();
        }
    }
    [HttpGet("{version}")]
    public IActionResult GetChangelogByQuery(string version)
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

            string currentVersion = result["current"].ToString();

            if (!result.ContainsKey(currentVersion))
            {
                return NotFound($"Version {currentVersion} not found.");
            }

            string currentChangelog = result[currentVersion].ToString();

            var response = new
            {
                version = currentVersion,
                changelog = currentChangelog
            };

            _logger.LogInformation("Version data loaded: " + JsonConvert.SerializeObject(response)); //System.Text.Json.JsonSerializer.Serialize(response));
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest();
        }
    }

}
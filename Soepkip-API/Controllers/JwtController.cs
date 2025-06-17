using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SoepkipAPI.Interfaces;

namespace SoepkipAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JwtController : Controller
{
    private readonly ILogger<TrashController> _logger;
    private readonly IJwtTokenGenerator _jwtGenerator;
    
    // GET: api/trash?dateLeft=a&dateRight=b
    public JwtController(ILogger<TrashController> logger, IJwtTokenGenerator jwtGenerator)
    {
        _logger = logger;
        _jwtGenerator = jwtGenerator;
    }

    [HttpGet]
    public IActionResult GetJwt([FromQuery] string key)
    {
        try
        {
            //Check if string contains anything
            if (key.IsNullOrEmpty()) throw new("No JWT was generated due to the supplied key being empty");
            
            //Generate the jwt key
            var jwtKey = _jwtGenerator.GenerateClientToken(key);
            
            //Check if a key was generated
            if (jwtKey.IsNullOrEmpty()) throw new("No JWT was generated due to an internal server error");

            return Ok(jwtKey);
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest($"Something went wrong while generating jwt: {e.Message}");
        }
    }
}
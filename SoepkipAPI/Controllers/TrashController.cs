using Microsoft.AspNetCore.Mvc;
using SoepkipAPI.Data.Interfaces;
using SoepkipAPI.Models;

namespace SoepkipAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TrashController : Controller
{
    //The repository where the trash data is handled
    private readonly ITrashRepository _trashRepository;
    
    //Used for logging within the docker container
    private readonly ILogger<TrashController> _logger;

    public TrashController(ITrashRepository trashRepository, ILogger<TrashController> logger)
    {
        _trashRepository = trashRepository;
        _logger = logger;
    }
    
    //GET: api/trash?dateLeft=a&dateRight=b
    [HttpGet]
    public IActionResult ReadRange(string dateLeft, string dateRight)
    {
        try
        {
            //Parse the left and right date to datetimes
            if (!DateTime.TryParse(dateLeft, out var dateLeftParsed)) throw new("Left date couldn't be parsed");
            if (!DateTime.TryParse(dateRight, out var dateRightParsed)) throw new("Right date couldn't be parsed");
            
            //Gets trash within range (inclusive)
            var trash = _trashRepository.ReadRange(dateLeftParsed, dateRightParsed);

            return Ok(trash);
        }
        catch (Exception e)
        {
            //Logs error and returns bad request
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest();
        }
    }
    
    //POST: api/trash
    [HttpPost]
    public async Task<IActionResult> Write([FromBody] TrashItem trash)
    {
        try
        {
            //Stages the writing to the data context
            _trashRepository.Write(trash);
            
            //Pushes the changes
           var rowsAffected = await _trashRepository.SaveChangesAsync();
           
            //In case nothing happened
            if (rowsAffected == 0) throw new("Writing trash to the context resulted in nothing happening");
            
            //Send back how many rows were affected (should always be 1)
            return Ok(rowsAffected);
        }
        catch (Exception e)
        {
            //Logs error and returns bad request
            _logger.LogError($"{e.Message}\n{e.InnerException}");
            return BadRequest();
        }
    }
}
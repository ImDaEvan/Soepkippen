using Microsoft.EntityFrameworkCore;

namespace SoepkipAPI.Models;

public class TrashItem
{
    public string id { get; set; }
    public DateTime timestamp { get; set; }
    public string type { get; set; }
    public float confidence { get; set; }
    public float longditude { get; set; }
    public float latitude { get; set; }
    
    //enriched values
    public float? feels_like_temp_celsius { get; set; }
    public float? actual_temp_celsius { get; set; }
    public float? wind_force_bft { get; set; }
    public float? wind_direction { get; set; }
}
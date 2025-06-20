using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace SoepkipAPI.Models;

public class TrashItem
{
    [Key]
    public string id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public DateTime timestamp { get; set; }
    
    [Required]
    public string type { get; set; }
    [Required]
    [Range(0,1)]
    public float confidence { get; set; }
    [Range(-180,180)]
    public float? longitude { get; set; }
    [Range(-90,90)]
    public float? latitude { get; set; }

    //enriched values
    public float? feels_like_temp_celsius { get; set; }
    public float? actual_temp_celsius { get; set; }
    public float? wind_force_bft { get; set; } 
    public float? wind_direction { get; set; }
    public DateTime? weather_timestamp { get; set; }
    
}
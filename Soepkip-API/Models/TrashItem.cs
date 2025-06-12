using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public float? feels_like_temp_celsius { get; set; }
    [JsonIgnore]
    public float? actual_temp_celsius { get; set; }
    [JsonIgnore]
    public float? wind_force_kmh { get; set; }
    [JsonIgnore]
    public string? wind_direction { get; set; }
    
}
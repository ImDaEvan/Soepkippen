public class WeatherResponse
{
    public List<WeatherData> liveweer { get; set; }
}

public class WeatherData
{
    public string Plaats { get; set; }
    public float Temp { get; set; }
    public float GTemp { get; set; }  // gevoelstemperatuur
    public float WindrGr { get; set; } // wind direction in degrees
    public float WindMs { get; set; } // wind speed in meter/second
    public float WindBft { get; set; } // wind speed in beaufort
    public string Time { get; set; }  // "12-06-2025 16:03"
    public string Timestamp { get; set; }  // Unix timestamp in seconds

    //Parsed DateTime from the time field
    public DateTime? ParsedTime => System.DateTime.TryParseExact(
        Time, "dd-MM-yyyy HH:mm", null,
        System.Globalization.DateTimeStyles.None, out var dt) ? dt : null;

    //Converted Unix timestamp
    public DateTime DateTime
    {
        get
        {
            if (long.TryParse(Timestamp, out var unix))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime;
            }
            return DateTime.MinValue; //unix epoch
        }
    }
}
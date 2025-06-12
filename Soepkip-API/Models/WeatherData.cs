public class WeatherResponse
{
    public List<WeatherData> liveweer { get; set; }
}

public class WeatherData
{
    public string plaats { get; set; }
    public float temp { get; set; }
    public float gtemp { get; set; }  // gevoelstemperatuur
    public float windrgr { get; set; }
    public float windms { get; set; }
    public string time { get; set; }  // "12-06-2025 16:03"
    public string timestamp { get; set; }  // Unix timestamp in seconds

    //Parsed DateTime from the time field
    public DateTime? ParsedTime => System.DateTime.TryParseExact(
        time, "dd-MM-yyyy HH:mm", null,
        System.Globalization.DateTimeStyles.None, out var dt) ? dt : null;

    //Converted Unix timestamp
    public DateTime DateTime
    {
        get
        {
            if (long.TryParse(timestamp, out var unix))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime;
            }
            return DateTime.MinValue; //unix epoch
        }
    }
}
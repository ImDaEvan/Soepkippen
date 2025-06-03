public class WeerLiveResponse
{
    public List<LiveWeather> liveweer { get; set; }
}

public class LiveWeather
{
    public string plaats { get; set; }
    public float temp { get; set; }
    public float gtemp { get; set; }  // gevoelstemperatuur
    public float windkmh { get; set; }
    public string windr { get; set; }
    public DateTime tijd { get; set; }
}

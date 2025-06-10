public class LiveWeather
{
    public string plaats { get; set; }
    public float temp { get; set; }
    public float gtemp { get; set; }  // gevoelstemperatuur
    public float windkmh { get; set; }
    public string windr { get; set; }
    public DateTime tijd { get; set; }
    public string datum { get; set; }

    public DateTime? Timestamp
    {
        get
        {
            try
            {
                return DateTime.ParseExact($"{datum} {tijd}", "yyyyMMdd HH:mm", null);
            }
            catch
            {
                return null;
            }
        }
    }
}
    


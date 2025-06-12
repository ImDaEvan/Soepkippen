using Newtonsoft.Json;
using SoepkipAPI.Models;

namespace SoepkipAPI.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "1cef529b51";

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherData?> GetWeatherAsync(string location)
    {
        var url = $"https://weerlive.nl/api/json-data-10min.php?locatie={location}&key={_apiKey}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var parsed = JsonConvert.DeserializeObject<WeatherResponse>(json);

        return parsed.liveweer.FirstOrDefault();
    }
}




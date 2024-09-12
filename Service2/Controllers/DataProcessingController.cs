using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedModels;

[ApiController]
[Route("[controller]")]
public class DataProcessingController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _service1Url;

    public DataProcessingController(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _service1Url = configuration["Service1Url"];
    }

    [HttpGet("aggregate")]
    public async Task<IEnumerable<string>> AggregateData()
    {
        var client = _clientFactory.CreateClient();
        var weatherResponse = await client.GetStringAsync($"{_service1Url}/weatherforecast");
        var userResponse = await client.GetStringAsync($"{_service1Url}/user");

        var weatherData = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(weatherResponse);
        var userData = JsonConvert.DeserializeObject<IEnumerable<User>>(userResponse);

        var aggregatedData = weatherData.Select(w => $"Weather: {w.Summary} on {w.Date.ToShortDateString()} with {w.TemperatureC}°C")
                                        .Concat(userData.Select(u => $"User: {u.Name}, Age: {u.Age}"));

        return aggregatedData;
    }

    [HttpPost("transform")]
    public async Task<string> TransformData([FromBody] WeatherForecast forecast)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.PostAsJsonAsync($"{_service1Url}/weatherforecast", forecast);
        var data = await response.Content.ReadAsStringAsync();

        return $"Transformed and posted: {data}";
    }
}
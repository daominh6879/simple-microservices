using Microsoft.AspNetCore.Mvc;
using SharedModels;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("{id}")]
    public ActionResult<WeatherForecast> Get(int id)
    {
        var rng = new Random();
        var forecast = new WeatherForecast
        {
            Date = DateTime.Now.AddDays(id),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        };
        return forecast;
    }

    [HttpPost]
    public ActionResult<WeatherForecast> Post([FromBody] WeatherForecast forecast)
    {
        // Simulate adding to a database
        return CreatedAtAction(nameof(Get), new { id = 1 }, forecast);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] WeatherForecast forecast)
    {
        // Simulate updating a database
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        // Simulate deleting from a database
        return NoContent();
    }
}
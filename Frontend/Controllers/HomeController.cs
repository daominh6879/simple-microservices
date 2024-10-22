using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using Newtonsoft.Json;
using SharedModels;

namespace Frontend.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _service2Url;

    public HomeController(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _service2Url = configuration["Service2Url"];
    }

    public async Task<IActionResult> Index()
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetStringAsync($"{_service2Url}/dataprocessing/aggregate");
        var data = JsonConvert.DeserializeObject<IEnumerable<string>>(response);
        return View(data);
    }
    public async Task<IActionResult> Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetStringAsync($"{_service2Url}/dataprocessing/aggregate/{id}");
        var data = JsonConvert.DeserializeObject<string>(response);
        return View("Details", data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(WeatherForecast forecast)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.PostAsJsonAsync($"{_service2Url}/dataprocessing/transform", forecast);
        var data = await response.Content.ReadAsStringAsync();
        return RedirectToAction("Index");
    }
}

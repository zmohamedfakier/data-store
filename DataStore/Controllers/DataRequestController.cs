using DataStore.Api.Series;
using DataStore.Services.TimeSeries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DataStore.Controllers
{
  [AllowAnonymous]
  [ApiController]
  [Route("[controller]")]
  public class DataRequestController : ControllerBase
  {
    private readonly ILogger<DataRequestController> _logger;
    private IServiceProvider _services;

    public DataRequestController(IServiceProvider services, ILogger<DataRequestController> logger)
    {
      _logger = logger;
      _services = services;
    }

    [HttpPost]
    [Route("/Timeseries")]
    public IActionResult TimeSeries([FromBody] JsonElement r)
    {
      TimeSeriesFunctionBody body = TimeSeriesFunctionJsonConverter.ConvertJsonRequest(r);

      TimeSeriesService service = _services.GetService<ITimeSeriesService>() as TimeSeriesService;

      TimeSeriesCollection timeSeriesCollection = service.Execute(body);
      string json = TimeSeriesCollectionJsonConverter.ToJson(timeSeriesCollection);
      return Content(json, "application/json");
    }

    [HttpPost]
    [Route("/Relationship")]
    public IActionResult Relationship([FromBody] dynamic requests, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string iteratorType)
    {
      return Ok(null);
    }
  }
}

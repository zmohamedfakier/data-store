using DataStore.Api;
using DataStore.Services.Dates;
using DataStore.Services.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataStore.Services.TimeSeries
{
  public class TimeSeriesFunctionRequest
  {
    public TimeSeriesFunctionRequest(IEnumerable<Tuple<string, string>> properties)
    {
      Properties = properties;
    }

    public string TimeSeries { get; set; }

    public string Source { get; set; }

    public IEnumerable<Tuple<string, string>> Properties { get; private set; }
  }

  public class TimeSeriesFunctionJsonConverter
  {
    public static TimeSeriesFunctionBody ConvertJsonRequest(JsonElement json)
    {
      TimeSeriesFunctionBody body = new TimeSeriesFunctionBody();
      body.Entities = GetEntities(json);
      body.Metrics = GetTimeSeries(json);
      body.Dates = GetDates(json);
      body.Options = GetOptions(json);

      return body;
    }

    private static Options GetOptions(JsonElement body)
    {
      Options options = new Options();
      options.RollForward = false;
      options.MatchPartialProperties = false;
      if (body.TryGetProperty("options", out JsonElement optionsJson) && optionsJson.ValueKind == JsonValueKind.Object)
      {
        options.RollForward = GetPropertyAsBoolean(optionsJson, "rollForward", false);
        options.MatchPartialProperties = GetPropertyAsBoolean(optionsJson, "partialProperties", false);
      }
      return options;
    }

    private static DateSetRequest GetDates(JsonElement body)
    {
      DateSetRequest dates = new DateSetRequest();
      dates.Start = dates.End = DateTime.Today.AddDays(-1.0);
      dates.Interval = "day";
      if (body.TryGetProperty("dates", out JsonElement dateJson) && dateJson.ValueKind == JsonValueKind.Object)
      {
        dates.Start = GetPropertyAsDateTime(dateJson, "start", DateTime.Today.AddDays(-1.0));
        dates.End = GetPropertyAsDateTime(dateJson, "end", DateTime.Today.AddDays(-1.0));
        dates.Interval = GetPropertyAsString(dateJson, "interval", "day");
      }

      return dates;
    }

    private static List<string> GetEntities(JsonElement body)
    {
      List<string> entities = new List<string>();
      if (body.TryGetProperty("entities", out JsonElement entityJson) && entityJson.ValueKind == JsonValueKind.Array)
      {
        foreach (JsonElement e in entityJson.EnumerateArray())
        {
          entities.Add(e.GetString());
        }
      }

      return entities;
    }

    private static List<TimeSeriesFunctionRequest> GetTimeSeries(JsonElement body)
    {
      List<TimeSeriesFunctionRequest> requests = new List<TimeSeriesFunctionRequest>();
      if (body.TryGetProperty("metrics", out JsonElement timeSeriesJson) && timeSeriesJson.ValueKind == JsonValueKind.Array)
      {
        foreach (JsonElement e in timeSeriesJson.EnumerateArray())
        {
          List<Tuple<string, string>> properties = new List<Tuple<string, string>>();
          TimeSeriesFunctionRequest tfr = new TimeSeriesFunctionRequest(properties);

          foreach (JsonProperty prop in e.EnumerateObject())
          {
            if (prop.Name == "timeseries")
              tfr.TimeSeries = prop.Value.GetString();
            else if (prop.Name == "source")
              tfr.Source = prop.Value.GetString();
            else if (prop.Name == "properties")
            {
              foreach (JsonElement propProp in prop.Value.EnumerateArray())
              {
                properties.Add(new Tuple<string, string>("unclassified", propProp.GetString()));
              }
            }
            else
              properties.Add(new Tuple<string, string>(prop.Name, prop.Value.GetString()));
          }

          requests.Add(tfr);
        }
      }

      return requests;
    }

    private static string GetPropertyAsString(JsonElement json, string property, string defaultValue)
    {
      if (!json.TryGetProperty(property, out JsonElement valueJson))
        return defaultValue;

      return valueJson.GetString();
    }

    private static DateTime GetPropertyAsDateTime(JsonElement json, string property, DateTime defaultValue)
    {
      if (!json.TryGetProperty(property, out JsonElement valueJson))
        return defaultValue;

      return DateTime.Parse(valueJson.GetString());
    }

    private static bool GetPropertyAsBoolean(JsonElement json, string property, bool defaultValue)
    {
      if (!json.TryGetProperty(property, out JsonElement valueJson))
        return defaultValue;

      if (valueJson.ValueKind == JsonValueKind.True)
        return true;

      if (valueJson.ValueKind == JsonValueKind.False)
        return false;

      if (valueJson.ValueKind == JsonValueKind.String)
        return Boolean.Parse(valueJson.GetString());

      throw new Exception($"The json type of {valueJson.ValueKind} cannot be converted to a boolean.");
    }
  }
}

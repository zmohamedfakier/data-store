using DataStore.Api;
using DataStore.Api.Series;
using DataStore.Api.Series.TimeSeries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.TimeSeries
{
  public class TimeSeriesCollectionJsonConverter
  {
    public static string ToJson(TimeSeriesCollection timeSeriesCollection)
    {
      StringWriter tw = new StringWriter();
      JsonTextWriter jtw = new JsonTextWriter(tw);
      if (timeSeriesCollection.Count > 1)
        jtw.WriteStartArray();

      foreach (ITimeSeries series in timeSeriesCollection)
      {
        jtw.WriteStartObject();
        jtw.WritePropertyName("entity");
        jtw.WriteValue(series.Entity.Code);
        jtw.WritePropertyName("timeSeries");
        WriteTimeSeries(jtw, series);
        WriteSeriesValues(jtw, series.Values);
        jtw.WriteEndObject();
      }

      if (timeSeriesCollection.Count > 1)
        jtw.WriteEndArray();

      jtw.Close();
      return tw.ToString();
    }

    private static void WriteSeriesValues(JsonTextWriter jtw, ISeriesValues values)
    {
      jtw.WritePropertyName("values");
      jtw.WriteStartArray();
      foreach (IDataPoint point in values)
      {
        point.Write(jtw);
      }
      jtw.WriteEndArray();
    }

    private static void WriteTimeSeries(JsonTextWriter jtw, ITimeSeries series)
    {
      jtw.WriteStartObject();
      jtw.WritePropertyName("timeSeries");
      jtw.WriteValue(series.TimeSeries.Code);
      jtw.WritePropertyName("source");
      jtw.WriteValue(series.Source.Name);

      DescriptorPropertySet dps = series.Properties;
      string[] unclassified = dps.GetLabels().ToArray();
      if (unclassified.Length > 0)
      {
        jtw.WritePropertyName("properties");
        jtw.WriteStartArray();
        foreach (string v in unclassified)
          jtw.WriteValue(v);
        jtw.WriteEndArray();
      }

      IEnumerable<KeyValuePair<string, string>> classified = dps.GetClassifiedProperties();
      foreach (KeyValuePair<string, string> prop in classified)
      {
        jtw.WritePropertyName(prop.Key.ToLower());
        jtw.WriteValue(prop.Value);
      }

      jtw.WriteEndObject();
    }
  }
}

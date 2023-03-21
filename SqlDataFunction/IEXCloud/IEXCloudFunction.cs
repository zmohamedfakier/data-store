using DataStore.Api;
using DataStore.Api.Series;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
  public class IEXCloudFunction : ITimeSeriesFunction
  {
    public TimeSeriesCollection Execute(IEnumerable<TimeSeriesRequest> requests, DateSet dateSet)
    {
      TimeSeriesCollection collection = new TimeSeriesCollection();

      Random r = new Random();

      foreach (TimeSeriesRequest request in requests)
      {
        NumericTimeSeries series = new NumericTimeSeries();
        for (DateTime current = dateSet.Start; current <= dateSet.End; current = current.AddDays(1.0))
        {
          series.Add(current, current, r.NextDouble() * 1000);
        }
        collection.Add(new TimeSeriesImplementation(request, series));
      }

      return collection;
    }
  }
}

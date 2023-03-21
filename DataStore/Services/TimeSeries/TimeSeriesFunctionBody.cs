using DataStore.Api;
using DataStore.Services.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.TimeSeries
{
  public class TimeSeriesFunctionBody
  {
    public IEnumerable<string> Entities { get; set; }
    public IEnumerable<TimeSeriesFunctionRequest> Metrics { get; set; }
    public DateSetRequest Dates { get; set; }
    public Options Options { get; set; }
  }
}

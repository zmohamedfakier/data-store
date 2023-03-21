using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Series
{
  public interface ITimeSeriesService
  {
    TimeSeriesCollection Execute(IEnumerable<TimeSeriesRequest> requests, DateSet dates, Options options);
  }
}

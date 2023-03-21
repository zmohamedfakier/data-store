using DataStore.Api.Series;
using System;
using System.Collections;
using System.Collections.Generic;


namespace DataStore.Api
{
  public interface ITimeSeriesFunction
  {
    TimeSeriesCollection Execute(IEnumerable<TimeSeriesRequest> requests, DateSet dateSet);
  }

}

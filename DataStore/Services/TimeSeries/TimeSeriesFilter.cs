using DataStore.Api;
using DataStore.Api.Series;
using DataStore.Api.Series.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.TimeSeries
{
  public class TimeSeriesFilter
  {
    internal static ITimeSeries Filter(ITimeSeries ts, DateSet dateSet, bool rollForward)
    {
      if (dateSet.Interval == Interval.Raw)
        return ExpandRawPoints(ts, dateSet);

      return ExpandDatePoints(ts, dateSet, rollForward);
    }

    private static ITimeSeries ExpandRawPoints(ITimeSeries ts, DateSet dateSet)
    {
      ExpandedTimeSeries ets = new ExpandedTimeSeries();
      foreach (IDataPoint dp in ts.Values)
      {
        if (dp.ValueDate < dateSet.Start || dp.ValueDate > dateSet.End)
          continue;

        ets.Add(dp);
      }

      TimeSeriesImplementation tsi = new TimeSeriesImplementation(new TimeSeriesRequest() { Entity = ts.Entity, TimeSeries = ts.TimeSeries, Properties = ts.Properties, Source = ts.Source }, ets);
      return tsi;
    }

    private static ITimeSeries ExpandDatePoints(ITimeSeries ts, DateSet dateSet, bool rollForward)
    {
      ExpandedTimeSeries ets = new ExpandedTimeSeries();
      ISeriesValues series = ts.Values;
      foreach (DateTime dt in dateSet.Dates)
      {
        IDataPoint point = series.GetPoint(dt, rollForward);
        ets.Add(point);
      }

      TimeSeriesImplementation tsi = new TimeSeriesImplementation(new TimeSeriesRequest() { Entity = ts.Entity, TimeSeries = ts.TimeSeries, Properties = ts.Properties, Source = ts.Source }, ets);
      return tsi;
    }
  }
}

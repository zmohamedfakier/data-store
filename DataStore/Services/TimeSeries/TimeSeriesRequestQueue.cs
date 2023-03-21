using DataStore.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using DataStore.Api.Series;

namespace DataStore.Services.TimeSeries
{
  public class TimeSeriesRequestQueue
  {
    private object padLock = new object();

    private Dictionary<ITimeSeriesFunction, ConcurrentHashSet<TimeSeriesRequest>> _Queue;

    public TimeSeriesRequestQueue()
    {
      _Queue = new Dictionary<ITimeSeriesFunction, ConcurrentHashSet<TimeSeriesRequest>>();
    }

    public void Queue(TimeSeriesRequest request, ITimeSeriesFunction function)
    {
      if (!_Queue.TryGetValue(function, out ConcurrentHashSet<TimeSeriesRequest> requests))
      {
        lock (padLock)
        {
          if (!_Queue.TryGetValue(function, out requests))
          {
            requests = new ConcurrentHashSet<TimeSeriesRequest>();
            _Queue.Add(function, requests);
          }
        }
      }

      requests.TryAdd(request);
    }

    public TimeSeriesCollection Process(DateSet dateSet, bool rollForward)
    {
      TimeSeriesCollection collection = new TimeSeriesCollection();

      foreach (KeyValuePair<ITimeSeriesFunction, ConcurrentHashSet<TimeSeriesRequest>> pr in _Queue)
      {
         ITimeSeriesFunction function = pr.Key;
         TimeSeriesCollection result = function.Execute(pr.Value, dateSet);

         foreach (TimeSeriesRequest request in pr.Value)
         {
            IEnumerable<ITimeSeries> series = result.Find(request);
            foreach (ITimeSeries ts in series)
            {
              ITimeSeries expanded = TimeSeriesFilter.Filter(ts, dateSet, rollForward);
              collection.Add(expanded);
            }
         }
      }

      return collection;
    }
  }
}

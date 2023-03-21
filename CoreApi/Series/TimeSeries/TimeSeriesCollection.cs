using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Series
{
  public class TimeSeriesCollection : IEnumerable<ITimeSeries>
  {
    private Dictionary<Tuple<int, int>, List<ITimeSeries>> _series;
    private int _count;

    public TimeSeriesCollection()
    {
      _series = new Dictionary<Tuple<int, int>, List<ITimeSeries>>();
      _count = 0;
    }

    public int Count
    {
      get
      {
        return _count;
      }
    }

    public void Add(TimeSeriesCollection result)
    {
      foreach (ITimeSeries series in result)
      {
        Add(series);
      }
    }

    public void Add(ITimeSeries series)
    {
      Tuple<int, int> key = new Tuple<int, int>(series.Entity.ID, series.TimeSeries.ID);
      if (!_series.TryGetValue(key, out List<ITimeSeries> matched))
      {
        matched = new List<ITimeSeries>();
        _series.Add(key, matched);
      }

      _count++;
      matched.Add(series);
    }

    public IEnumerator<ITimeSeries> GetEnumerator()
    {
      foreach (List<ITimeSeries> tsList in _series.Values)
      {
        foreach (ITimeSeries ts in tsList)
        {
          yield return ts;
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (List<ITimeSeries> tsList in _series.Values)
      {
        foreach (ITimeSeries ts in tsList)
        {
          yield return ts;
        }
      }
    }

    public IEnumerable<ITimeSeries> Find(TimeSeriesRequest request)
    {
      if (!_series.TryGetValue(new Tuple<int, int>(request.Entity.ID, request.TimeSeries.ID), out List<ITimeSeries> matches))
        yield break;

      foreach (ITimeSeries match in matches)
      {
        if (IsMatch(match, request))
          yield return match;
      }
    }

    private bool IsMatch(ITimeSeries match, TimeSeriesRequest request)
    {
      if (request.Source != null && request.Source.ID != match.Source.ID)
        return false;

      return request.Properties.Match(match.Properties);
    }
  }
}

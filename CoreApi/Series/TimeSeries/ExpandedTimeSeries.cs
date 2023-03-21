using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Series.TimeSeries
{
  public class ExpandedTimeSeries : ISeriesValues
  {
    List<IDataPoint> _points = new List<IDataPoint>();

    public IDataPoint GetPoint(DateTime date, bool rollForward = true)
    {
      return _points.FirstOrDefault(t => t.ValueDate == date);
    }

    public void Add(IDataPoint point)
    {
      _points.Add(point);
    }

    public IEnumerator<IDataPoint> GetEnumerator()
    {
      return _points.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _points.GetEnumerator();
    }
  }
}

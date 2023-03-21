using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Series
{
  public interface IValues<T> : IEnumerable<KeyValuePair<DateTime, T>>
  {
    void Add(DateTime declarationDate, T value);
  }

  public class SingleValue<T> : IValues<T>, IEnumerable<KeyValuePair<DateTime, T>>
  {
    private KeyValuePair<DateTime, T> _value;

    public IEnumerator<KeyValuePair<DateTime, T>> GetEnumerator()
    {
      return _value.AsEnumerable();
    }

    public void Add(DateTime declarationDate, T value)
    {
      _value = new KeyValuePair<DateTime, T>(declarationDate, value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _value.AsEnumerable();
    }
  }

  public class RevisableValue<T> : IValues<T>, IEnumerable<KeyValuePair<DateTime, T>>
  {
    private SortedList<DateTime, T> _revisableValues;

    public RevisableValue()
    {
      _revisableValues = new SortedList<DateTime, T>();
    }

    public void Add(DateTime declarationDate, T value)
    {
      _revisableValues[declarationDate] = value;
    }

    public IEnumerator<KeyValuePair<DateTime, T>> GetEnumerator()
    {
      return _revisableValues.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _revisableValues.GetEnumerator();
    }
  }

  public class GenericSeries<T> : ITimeSeriesValues
  {
    private SortedList<DateTime, IValues<T>> _values;

    public GenericSeries()
    {
      _values = new SortedList<DateTime, IValues<T>>();
    }

    public IDataPoint GetPoint(DateTime date, bool rollForward = true)
    {
      if (!_values.TryGetValue(date, out IValues<T> values))
      {
        if (!rollForward)
          return new GenericPoint(date, date, null);

        DateTime previous = _values.Keys.GetPreviousDate(date);
        if (previous == DateTime.MinValue)
          return new GenericPoint(date, date, null);

        values = _values[previous];
      }

      KeyValuePair<DateTime, T> lastPoint = values.Last();
      return new GenericPoint(date, lastPoint.Key, lastPoint.Value);
    }

    public void Add(DateTime valueDate, DateTime declarationDate, object value)
    {
      Add(valueDate, declarationDate, (T)value);
    }

    public void Add(DateTime valueDate, DateTime declarationDate, T value)
    {
      if (_values.TryGetValue(valueDate, out IValues<T> existing))
      {
        RevisableValue<T> revisable = existing as RevisableValue<T>;
        if (revisable == null)
        {
          revisable = new RevisableValue<T>();
          foreach (KeyValuePair<DateTime, T> vals in existing)
            revisable.Add(vals.Key, vals.Value);

        }
        else
        {

        }
        SingleValue<T> dv = new SingleValue<T>();
        dv.Add(declarationDate, value);
        _values[valueDate] = dv;
      }
      else
      {
        SingleValue<T> dv = new SingleValue<T>();
        dv.Add(declarationDate, value);
        _values[valueDate] = dv;
      }
    }

    public IEnumerator<IDataPoint> GetEnumerator()
    {
      foreach (KeyValuePair<DateTime, IValues<T>> pr in _values)
      {
        foreach (KeyValuePair<DateTime, T> vals in pr.Value)
        {
          yield return new GenericPoint(pr.Key, vals.Key, vals.Value);
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }
}

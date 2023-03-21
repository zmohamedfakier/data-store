using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Series.Static
{
  public abstract class BaseStatic<T> : IStaticSeriesValues
  {
    protected T _value;

    public BaseStatic()
    {
    }

    public IDataPoint GetPoint(DateTime date, bool rollForward = true)
    {
      return new GenericPoint(date, date, _value);
    }

    public IEnumerator<IDataPoint> GetEnumerator()
    {
      yield return new GenericPoint(DateTime.MinValue, DateTime.MinValue, _value);
    }

    public void SetValue(T value)
    {
      _value = value;
    }

    public void SetValue(object value)
    {
      _value = (T)value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      yield return new GenericPoint(DateTime.MinValue, DateTime.MinValue, _value);
    }
  }

  public class TextStatic : BaseStatic<string>
  {
  }
}

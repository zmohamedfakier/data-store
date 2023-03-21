using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  [Flags]
  public enum Interval
  {
    Trading,
    Day,
    MonthStart,
    MonthEnd,
    Raw
  }

  public class DateSet
  {
    public DateSet(DateTime start, DateTime end, Interval type)
      : this(start, end, type, Enumerable.Empty<DateTime>())
    {

    }

    public DateSet(DateTime start, DateTime end, Interval type, IEnumerable<DateTime> dates)
    {
      Start = start;
      End = end;
      Interval = type;
      Dates = dates;
    }

    public DateTime Start { get; private set; }

    public DateTime End { get; private set; }

    public Interval Interval { get; private set; }

    public IEnumerable<DateTime> Dates { get; private set; }
  }
}

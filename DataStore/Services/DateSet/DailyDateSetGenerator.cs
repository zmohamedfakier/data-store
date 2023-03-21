using DataStore.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.Dates
{
  public class RawDateSetGenerator : IDateSetGenerator
  {
    public DateSet GenerateDates(DateTime start, DateTime end, Interval interval)
    {
      return new DateSet(start, end, interval, new DateTime[0]);
    }
  }

  public class DailyDateSetGenerator : IDateSetGenerator
  {
    public DateSet GenerateDates(DateTime start, DateTime end, Interval interval)
    {
      DateTime[] dates = new DateTime[(int)Math.Floor(end.Subtract(start).TotalDays + 1.0)];
      int i = 0;
      for(DateTime dt = start; dt <= end; dt = dt.AddDays(1.0), ++i)
      {
        dates[i] = dt;
      }

      return new DateSet(start, end, interval, dates);
    }
  }

  public class TradingDaySetGenerator : IDateSetGenerator
  {
    public DateSet GenerateDates(DateTime start, DateTime end, Interval interval)
    {
      List<DateTime> dates = new List<DateTime>((int)Math.Floor(end.Subtract(start).TotalDays + 1.0));
      int i = 0;
      for (DateTime dt = start; dt <= end; dt = dt.AddDays(1.0), ++i)
      {
        if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
          continue;

        dates.Add(dt);
      }

      return new DateSet(start, end, interval, dates);
    }
  }

  public class MonthlyStartSetGenerator : IDateSetGenerator
  {
    public DateSet GenerateDates(DateTime start, DateTime end, Interval interval)
    {
      List<DateTime> dates = new List<DateTime>((int)Math.Floor(end.Subtract(start).TotalDays/28)+1);
      int i = 0;
      DateTime nextMonthStart = start;
      if (nextMonthStart.Day != 1)
      {
        nextMonthStart = start.AddMonths(1);
        nextMonthStart = new DateTime(nextMonthStart.Year, nextMonthStart.Month, 1);
      }

      for (DateTime dt = nextMonthStart; dt <= end; dt = dt.AddMonths(1), ++i)
      {
        dates.Add(dt);
      }

      return new DateSet(start, end, interval, dates);
    }
  }

  public class MonthlyEndSetGenerator : IDateSetGenerator
  {
    public DateSet GenerateDates(DateTime start, DateTime end, Interval interval)
    {
      List<DateTime> dates = new List<DateTime>((int)Math.Floor(end.Subtract(start).TotalDays/28)+1);
      int i = 0;
      DateTime nextMonthEnd = start;
      int lastDayInMonth = DateTime.DaysInMonth(nextMonthEnd.Year, nextMonthEnd.Month);
      if (nextMonthEnd.Day != lastDayInMonth)
      {
        nextMonthEnd = new DateTime(nextMonthEnd.Year, nextMonthEnd.Month, lastDayInMonth);
      }

      for (DateTime dt = nextMonthEnd; dt <= end; ++i)
      {
        dates.Add(dt);
        dt = dt.AddDays(dt.Month == 12 ? 31 : DateTime.DaysInMonth(dt.Year, dt.Month + 1));
      }

      return new DateSet(start, end, interval, dates);
    }
  }



}

using DataStore.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.Dates
{
  public class DateSetService : IDateSetService
  {
    private Dictionary<Interval, IDateSetGenerator> _generators;

    public DateSetService()
    {
      _generators = new Dictionary<Interval, IDateSetGenerator>
      {
        { Interval.Day, new DailyDateSetGenerator() },
        { Interval.Trading, new TradingDaySetGenerator() },
        { Interval.MonthStart, new MonthlyStartSetGenerator() },
        { Interval.MonthEnd, new MonthlyEndSetGenerator() },
        { Interval.Raw, new RawDateSetGenerator() }
      };
    }

    public DateSet GetDateSet(DateTime start, DateTime end, string interval)
    {
      if (!Enum.TryParse(interval, true, out Interval validInterval))
        throw new ArgumentException("The interval specified is invalid", "interval");

      if (!_generators.TryGetValue(validInterval, out IDateSetGenerator generator))
        throw new ArgumentException("The interval specified is invalid", "interval");

      return generator.GenerateDates(start, end, validInterval);
    }


  }
}

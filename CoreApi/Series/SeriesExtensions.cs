using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Series
{
  public static class SeriesExtensions
  {
    public static IEnumerator<T> AsEnumerable<T>(this T item)
    {
      yield return item;
    }

    public static DateTime GetPreviousDate(this IList<DateTime> list, DateTime date)
    {
      if (list.Count == 0)
        return DateTime.MinValue;

      int lo = 0, hi = list.Count - 1;
      while (lo < hi)
      {
        int m = (hi + lo) / 2;  // this might overflow; be careful.
        if (list[m].CompareTo(date) < 0)
          lo = m + 1;
        else
          hi = m - 1;
      }

      if (list[lo].CompareTo(date) > 0)
        lo--;

      if (lo < 0)
        return DateTime.MinValue;

      return list[lo];
    }
  }
}

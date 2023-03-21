using DataStore.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.Dates
{
  public interface IDateSetGenerator
  {
    DateSet GenerateDates(DateTime start, DateTime end, Interval interval);
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.Dates
{
  public class DateSetRequest
  {
    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string Interval { get; set; }
  }
}

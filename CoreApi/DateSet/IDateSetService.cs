using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public interface IDateSetService
  {
    DateSet GetDateSet(DateTime start, DateTime end, string iteratorType);
  }
}

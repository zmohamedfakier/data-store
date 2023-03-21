using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public interface IFunctionFactoryService
  {
    T GetFunction<T>(int functionID) where T : class;
  }
}

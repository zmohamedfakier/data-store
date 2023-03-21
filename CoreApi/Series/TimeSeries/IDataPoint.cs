using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Series
{
  public interface IDataPoint
  {
    DateTime ValueDate { get; }

    DateTime DeclarationDate { get; }

    object Data { get; }

    void Write(JsonWriter wt);
  }
}

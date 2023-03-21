using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Series
{
  public class GenericPoint : IDataPoint
  {
    public GenericPoint(DateTime valueDate, DateTime declarationDate, object value)
    {
      this.ValueDate = valueDate;
      this.DeclarationDate = declarationDate;
      this.Data = value;
    }

    public object Data { get; private set; }

    public DateTime ValueDate { get; private set; }

    public DateTime DeclarationDate { get; private set; }

    public void Write(JsonWriter wt)
    {
        wt.WriteStartObject();
        wt.WritePropertyName("Value Date");
        wt.WriteValue(ValueDate.ToString("dd MMM yyy"));
        wt.WritePropertyName("Declaration Date");
        wt.WriteValue(DeclarationDate.ToString("dd MMM yyy"));
        wt.WritePropertyName("Value");
        wt.WriteValue(Data?.ToString() ?? "null");
        wt.WriteEndObject();
    }
  }
}

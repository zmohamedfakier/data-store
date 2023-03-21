using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Series
{
  public interface ITimeSeries
  {
    IEntityDescriptor Entity { get; }
    
    IEntityDescriptor TimeSeries { get; }

    ISourceDescriptor Source { get; }

    DescriptorPropertySet Properties { get; }

    ISeriesValues Values { get; }
  }

  public interface ISeriesValues : IEnumerable<IDataPoint>
  {
    IDataPoint GetPoint(DateTime date, bool rollForward = true);
  }

  public interface ITimeSeriesValues : ISeriesValues
  {
    void Add(DateTime valueDate, DateTime declarationDate, object value);
  }

  public interface IStaticSeriesValues : ISeriesValues
  {
    void SetValue(object value);
  }
}

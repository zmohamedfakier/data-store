using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Series
{
  public class TimeSeriesImplementation : ITimeSeries
  {
    private TimeSeriesRequest _request;
    private ISeriesValues _values;

    public TimeSeriesImplementation(TimeSeriesRequest request, ISeriesValues values)
    {
      _request = request;
      _values = values;
    }

    public IEntityDescriptor Entity { get { return _request.Entity; } }
    public IEntityDescriptor TimeSeries { get { return _request.TimeSeries; } }
    public ISourceDescriptor Source { get { return _request.Source; } }
    public DescriptorPropertySet Properties { get { return _request.Properties; } }
    public ISeriesValues Values { get { return _values; } }

  }
}

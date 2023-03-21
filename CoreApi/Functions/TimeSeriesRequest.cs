using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public class TimeSeriesRequest
  {
    public IEntityDescriptor Entity { get; set; }
    public IEntityDescriptor TimeSeries { get; set; }
    public DescriptorPropertySet Properties { get; set; }
    public ISourceDescriptor Source { get; set; }
    public int DescriptorID { get; set; }
  }
}

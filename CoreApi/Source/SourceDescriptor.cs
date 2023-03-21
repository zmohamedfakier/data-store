using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public class SourceDescriptor : ISourceDescriptor
  {
    public int ID { get; set; }

    public string Name { get; set; }
  }
}

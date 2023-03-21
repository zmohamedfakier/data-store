using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public interface ISourceDescriptor
  {
    int ID { get; }

    public string Name { get; }
  }
}

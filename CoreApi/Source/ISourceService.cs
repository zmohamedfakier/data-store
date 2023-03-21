using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public interface ISourceService
  {
    ISourceDescriptor GetSource(string source);

    ISourceDescriptor GetSource(int source);
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public interface IEntityDescriptor
  {
    int ID { get; }

    string Code { get; }

    string CodeSet { get; }

    int CodeSetID { get; }

    IEntityDescriptorCollection Codes { get; }
    
  }
}

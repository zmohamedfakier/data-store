using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public interface IEntityDescriptorCollection : IEnumerable<IEntityDescriptor>
  {
    IEntityDescriptor this[string codeType] { get; }

    IEntityDescriptor this[int index] { get; }

    void Add(IEntityDescriptor descriptor);
  }
}

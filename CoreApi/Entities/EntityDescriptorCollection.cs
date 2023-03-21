using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Entities
{
  public class EntityDescriptorCollection : IEntityDescriptorCollection
  {
    private Dictionary<int, IEntityDescriptor> _EntityDescriptors;
    private Dictionary<string, int> _CodeSetLookup;

    public EntityDescriptorCollection(Dictionary<string, int> codeSets)
    {
      _CodeSetLookup = codeSets;
      _EntityDescriptors = new Dictionary<int, IEntityDescriptor>();
    }

    public void Add(IEntityDescriptor descriptor)
    {
      _EntityDescriptors.Add(descriptor.CodeSetID, descriptor);
    }

    public IEntityDescriptor this[string codeType]
    {
      get 
      {
        if (!_CodeSetLookup.TryGetValue(codeType, out int codeTypeID))
          throw new Exception($"The code type {codeType} does not exist.");

        return this[codeTypeID];
      }
    }

    public IEntityDescriptor this[int index]
    {
      get
      {
        if (_EntityDescriptors.TryGetValue(index, out IEntityDescriptor entity))
          return entity;

        return null;
      }
    }

    public IEnumerator<IEntityDescriptor> GetEnumerator()
    {
      return _EntityDescriptors.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _EntityDescriptors.Values.GetEnumerator();
    }
  }
}

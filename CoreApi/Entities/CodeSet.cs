using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public class CodeSet
  {
    public int ID { get; }

    public string Name { get; }

    public Dictionary<string, IEntityDescriptor> Members { get; private set; }

    public CodeSet(int id, string name)
    {
      ID = id;
      Name = name;
      Members = new Dictionary<string, IEntityDescriptor>(StringComparer.OrdinalIgnoreCase);

    }

    public bool TryGetValue(string entityCode, out IEntityDescriptor entityDescriptor)
    {
      return Members.TryGetValue(entityCode, out entityDescriptor);
    }

    public void Add(IEntityDescriptor descriptor)
    {
      Members.Add(descriptor.Code, descriptor);
    }
  }
}

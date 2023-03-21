using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.Properties
{
  public class PropertySetRequest
  {
    private HashSet<string> _unclassifiedProperties;
    private Dictionary<string, string> _classifiedProperties;

    public PropertySetRequest()
    {
      _unclassifiedProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      _classifiedProperties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public void Add(string property)
    {
      _unclassifiedProperties.Add(property);
    }

    public void Add(string classification, string property)
    {
      _classifiedProperties[classification] = property;
    }

    public IEnumerable<string> UnclassifiedProperties { get { return _unclassifiedProperties; } }

    public IEnumerable<KeyValuePair<string, string>> ClassifiedProperties { get { return _classifiedProperties; } }
  }
}

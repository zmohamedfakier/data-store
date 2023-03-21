using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public enum PropertySetType
  {
    Unclassified = 1,
    ValueSet = 2,
    EntitySet = 3
  }

  public class PropertySet : IEnumerable<PropertyValue>
  {
    private Dictionary<string, PropertyValue> _properties;

    public string Name { get; private set; }
    public int ID { get; private set; }
    public PropertySetType Type { get; set; }
    public int? CodeSetID { get; set; }

    public PropertySet(int id, string name, PropertySetType type, int? codeSetID)
    {
      ID = id;
      Name = name;
      Type = type;
      CodeSetID = codeSetID;
      _properties = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);
    }

    public IEnumerator<PropertyValue> GetEnumerator()
    {
      return _properties.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _properties.Values.GetEnumerator();
    }

    public bool TryGetValue(string value, out PropertyValue pv)
    {
      return _properties.TryGetValue(value, out pv);
    }

    public void Add(PropertyValue pv)
    {
      _properties.Add(pv.Value, pv);
    }
  }
}

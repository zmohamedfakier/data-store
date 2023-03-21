using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public class DescriptorPropertyValue
  {
    public int PropertySetID { get; set; }
    public int EntityID { get; set; }
    public int ValueID { get; set; }
    public string PropertySet { get; set; }
    public string Value { get; set; }
  }

  public class DescriptorPropertySet : IEnumerable<DescriptorPropertyValue>
  {
    private static DescriptorPropertySet _empty;

    static DescriptorPropertySet()
    {
      _empty = new DescriptorPropertySet(new DescriptorPropertyValue[0]); 
    }

    public static DescriptorPropertySet Empty { get { return _empty; } }

    private IEnumerable<DescriptorPropertyValue> _values;

    public DescriptorPropertySet(IEnumerable<DescriptorPropertyValue> values)
    {
      _values = values;
    }

    public IEnumerator<DescriptorPropertyValue> GetEnumerator()
    {
      return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _values.GetEnumerator();
    }

    public IEnumerable<string> GetLabels()
    {
      return _values.Where(t => t.PropertySetID == 0).Select<DescriptorPropertyValue, string>(t => t.Value);
    }

    public IEnumerable<KeyValuePair<string, string>> GetClassifiedProperties()
    {
      return _values.Where(t => t.PropertySetID != 0).Select<DescriptorPropertyValue, KeyValuePair<string, string>>(t => new KeyValuePair<string, string>(t.PropertySet, t.Value));
    }

    public bool Match(DescriptorPropertySet properties)
    {
      // Check that all the properties in this set exist in the properties
      foreach (DescriptorPropertyValue dpv in _values)
      {
        DescriptorPropertyValue found = properties._values.FirstOrDefault(t => t.PropertySetID == dpv.PropertySetID && t.ValueID == dpv.ValueID && t.EntityID == dpv.EntityID);
        if (found == null)
          return false;
      }

      return true;
    }

    
  }
}

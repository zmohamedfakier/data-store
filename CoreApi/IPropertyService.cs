using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public interface IPropertyService
  {
    DescriptorPropertySet GetPropertySet(IEnumerable<Tuple<string, string>> classifiedProperties);

    DescriptorPropertySet GetPropertySet(int id);
  }
}

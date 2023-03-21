using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public class PropertyValue
  {
    public int ID { get; set; }

    public string Value { get; set; }

    public int? EntityID { get; set; }
  }
}

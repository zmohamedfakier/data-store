using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Relationships
{
  public class RelationshipSeriesFunctionRequest
  {
    public string Entity { get; set; }

    public string TimeSeries { get; set; }

    public string Source { get; set; }

    public IEnumerable<KeyValuePair<string, string>> Attributes { get; set; }
  }
}

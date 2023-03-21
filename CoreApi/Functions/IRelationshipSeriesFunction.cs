using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public class RelationshipSeriesRequest
  {
    public IEntityDescriptor Entity { get; set; }
    public IEntityDescriptor Relationship { get; set; }
    public IEnumerable<DescriptorPropertySet> Properties { get; set; }
    public ISourceDescriptor Source { get; set; }
  }

  public class RelationshipSeriesCollection
  {
  }

  public class IRelationshipSeriesFunction
  {
    RelationshipSeriesCollection Calculate(IEnumerable<RelationshipSeriesRequest> requests, DateSet dateSet)
    {
      return null;
    }
  }
}

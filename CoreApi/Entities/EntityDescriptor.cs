using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api.Entities
{
  public class EntityDescriptor : IEntityDescriptor
  {
    public EntityDescriptor(int id, string code, int codeSetId, string codeType, IEntityDescriptorCollection collection)
    {
      ID = id;
      Code = code;
      CodeSetID = codeSetId;
      CodeSet = codeType;
      Codes = collection;
    }

    public int ID { get; private set; }

    public string Code { get; private set; }

    public int CodeSetID { get; private set; }

    public string CodeSet { get; private set; }

    public IEntityDescriptorCollection Codes { get; private set; }
  }
}

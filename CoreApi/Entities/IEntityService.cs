using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public interface IEntityService
  {
    IEntityDescriptor GetEntity(string entity);

    IEntityDescriptor GetEntity(string entity, int codeSetID);
  }
}

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore.Api
{
  public class ConcurrentHashSet<T> : IEnumerable<T>
  {
    private ConcurrentDictionary<T, byte> _HashSet = new ConcurrentDictionary<T, byte>();

    public bool TryAdd(T key)
    {
      return _HashSet.TryAdd(key, Byte.MinValue);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return _HashSet.Keys.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _HashSet.Keys.GetEnumerator();
    }
  }
}

using GroupsReact.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GroupsReact.Helpers
{
  public class InMemoryUserCache
  {
    private static readonly object MemLock = new object();
    private readonly string _cacheId;
    private readonly IMemoryCache _memoryCache;
    private UserModel _cache = new UserModel();

    public InMemoryUserCache(string userId, IMemoryCache memoryCache)
    {
      _cacheId = userId + "_UserCache";
      _memoryCache = memoryCache;
    }

    public void SaveUserStateValue(UserModel state)
    {
      lock (MemLock)
      {
        _memoryCache.Set(_cacheId + "_state", state);
      }
    }

    public UserModel ReadUserStateValue()
    {
      UserModel state;
      lock (MemLock)
      {
        state = _memoryCache.Get(_cacheId + "_state") as UserModel;
      }
      return state;
    }


    // Empties the persistent store.
    public void Clear()
    {
      _cache = null;
      lock (MemLock)
      {
        _memoryCache.Remove(_cacheId);
      }
    }
  }
}

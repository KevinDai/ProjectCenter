using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace ProjectCenter.Util
{
    public class Cache : ICache
    {
        private static ICache _instance = new Cache();
        public static ICache Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private ObjectCache _cache;

        private Cache()
        {
            _cache = MemoryCache.Default;
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        //public T Get<T>(string key)
        //{
        //    return (T)_cache.Get(key);
        //}

        public void Set(string key, object value)
        {
            Set(key, value, 10);
        }

        public void Set(string key, object value, int minute)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            //policy.SlidingExpiration = DateTime.Now.AddMinutes(minute); //相对时间过期
            policy.AbsoluteExpiration = DateTime.Now.AddMinutes(minute); //绝对时间过期
            _cache.Set(key, value, policy);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}

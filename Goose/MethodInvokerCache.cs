using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Goose
{
    class MethodInvokerCache
    {
        private static readonly HashSet<MethodInvokerCacheKey> _blacklist;
        private static readonly Dictionary<MethodInvokerCacheKey, Func<object, object[], object>> _knownHandlers;
        private static readonly ReaderWriterLockSlim _lock;

        static MethodInvokerCache()
        {
            _blacklist = new HashSet<MethodInvokerCacheKey>();
            _knownHandlers = new Dictionary<MethodInvokerCacheKey, Func<object, object[], object>>();
            _lock = new ReaderWriterLockSlim();
        }

        public static Func<object, object[], object> GetHandler(MethodInfo method, GooseOptions options, out bool blacklist)
        {
            _lock.EnterReadLock();
            try
            {
                var key = new MethodInvokerCacheKey(method, options);
                blacklist = _blacklist.Contains(key);
                if (!blacklist && _knownHandlers.ContainsKey(key)) return _knownHandlers[key];
                return null;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public static void AddHandler(MethodInfo method, GooseOptions options, Func<object, object[], object> handler)
        {
            _lock.EnterWriteLock();
            var key = new MethodInvokerCacheKey(method, options);
            _knownHandlers.Add(key, handler);
            _lock.ExitWriteLock();
        }

        public static void AddBlacklist(MethodInfo method, GooseOptions options)
        {
            _lock.EnterWriteLock();
            var key = new MethodInvokerCacheKey(method, options);
            _blacklist.Add(key);
            _lock.ExitWriteLock();
        }
    }
}

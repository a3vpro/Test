using System;
using System.Collections.Concurrent;
using System.Threading;

namespace VisionNet.Core.Threading
{
    public static class LockExtensions
    {
        private static readonly ConcurrentDictionary<object, string> LockInformation = new ConcurrentDictionary<object, string>();

        public static void Lock<T>(this T target, Action<T> action, string callerInformation)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (action == null) throw new ArgumentNullException(nameof(action));

            lock (target)
            {
                try
                {
                    LockInformation[target] = callerInformation;
                    action(target);
                }
                finally
                {
                    LockInformation.TryRemove(target, out _);
                }
            }
        }

        public static string GetLockInformation<T>(this T target)
        {
            LockInformation.TryGetValue(target, out var info);
            return info;
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace VisionNet.Core.Threading
{
    public static class LockExtensions
    {
        private static readonly ConcurrentDictionary<object, string> LockInformation = new ConcurrentDictionary<object, string>();

        /// <summary>
        /// Acquires a monitor lock on <paramref name="target"/> and executes <paramref name="action"/> while recording caller-specific diagnostic information.
        /// </summary>
        /// <typeparam name="T">The type of the object used as the synchronization target.</typeparam>
        /// <param name="target">The object whose monitor is locked. This parameter must not be <see langword="null"/>.</param>
        /// <param name="action">The delegate invoked while the lock is held. This parameter must not be <see langword="null"/>.</param>
        /// <param name="callerInformation">A descriptive identifier for the lock owner recorded for diagnostics while the lock is held.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> or <paramref name="action"/> is <see langword="null"/>.</exception>
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

        /// <summary>
        /// Retrieves the diagnostic information recorded for the current holder of a lock on <paramref name="target"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object used as the synchronization target.</typeparam>
        /// <param name="target">The object whose recorded lock information is requested.</param>
        /// <returns>The diagnostic information associated with the lock if it is currently held; otherwise, <see langword="null"/>.</returns>
        public static string GetLockInformation<T>(this T target)
        {
            LockInformation.TryGetValue(target, out var info);
            return info;
        }
    }
}

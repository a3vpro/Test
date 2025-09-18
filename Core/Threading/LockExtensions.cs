using System;
using System.Collections.Concurrent;
using System.Threading;

namespace VisionNet.Core.Threading
{
    public static class LockExtensions
    {
        private static readonly ConcurrentDictionary<object, string> LockInformation = new ConcurrentDictionary<object, string>();

        /// <summary>
        /// Executes the provided <paramref name="action"/> while holding a mutual-exclusion lock on
        /// <paramref name="target"/>, storing the supplied caller information for diagnostic purposes
        /// during the locked execution.
        /// </summary>
        /// <typeparam name="T">The type of the object used as the synchronization target.</typeparam>
        /// <param name="target">
        /// The instance whose monitor lock is acquired before invoking <paramref name="action"/>.
        /// This object must be shared by all threads that need to coordinate access.
        /// </param>
        /// <param name="action">
        /// A delegate that receives <paramref name="target"/> and is executed while the lock is held.
        /// The delegate should avoid long-running work to prevent blocking other threads.
        /// </param>
        /// <param name="callerInformation">
        /// Contextual information about the caller (for example, the member name or stack frame) that
        /// is associated with the lock while it is held; may be <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="target"/> or <paramref name="action"/> is <see langword="null"/>.
        /// </exception>
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
        /// Retrieves the most recent caller information that was recorded while a lock was held on the
        /// specified <paramref name="target"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the object whose lock metadata is requested.</typeparam>
        /// <param name="target">
        /// The instance previously used in <see cref="Lock{T}(T, Action{T}, string)"/> for which the stored
        /// caller information is requested.
        /// </param>
        /// <returns>
        /// The caller information associated with the current or most recent lock on <paramref name="target"/>,
        /// or <see langword="null"/> if no information is available.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is <see langword="null"/>.</exception>
        public static string GetLockInformation<T>(this T target)
        {
            LockInformation.TryGetValue(target, out var info);
            return info;
        }
    }
}

using System;
using System.Collections.Generic;

namespace VisionNet.Core.Threading
{
    /// <summary>
    /// Provides a synchronization primitive that serializes access to actions identified by a shared identifier and releases
    /// the identifier-specific lock when disposed.
    /// </summary>
    public class TransactionalAction: IDisposable
    {
        private readonly object _lockObject;
        private static readonly Dictionary<string, object> _lockObjects = new Dictionary<string, object>();
        private static readonly object _lockCreationObject = new object();
        private readonly string _id;
        private bool disposed = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalAction"/> class for the specified identifier.
        /// </summary>
        /// <param name="id">An identifier used to group actions that must not execute concurrently.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is <see langword="null"/>.</exception>
        public TransactionalAction(string id)
        {
            _id = id;
            _lockObject = GetLockObject(id);
        }

        private static object GetLockObject(string id)
        {
            lock (_lockCreationObject)
            {
                if (!_lockObjects.ContainsKey(id))
                {
                    _lockObjects[id] = new object();
                }

                return _lockObjects[id];
            }
        }

        /// <summary>
        /// Executes the provided action while holding the lock associated with this instance's identifier, ensuring exclusive execution among peers that share the same identifier.
        /// </summary>
        /// <param name="action">A delegate to invoke while the identifier-specific lock is held.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="Exception">Any exception thrown by <paramref name="action"/> propagates to the caller.</exception>
        public void Execute(Action action)
        {
            lock (_lockObject)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Releases resources associated with this instance and removes the identifier-specific lock, allowing future instances with the same identifier to create a new lock object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases managed resources associated with the instance, removing the identifier-specific lock when invoked from
        /// <see cref="Dispose()"/>.
        /// </summary>
        /// <param name="disposing">Indicates whether the method is invoked from <see cref="Dispose()"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    lock (_lockCreationObject)
                    {
                        _lockObjects.Remove(_id);
                    }
                }

                disposed = true;
            }
        }

        ~TransactionalAction()
        {
            Dispose(false);
        }
    }
}

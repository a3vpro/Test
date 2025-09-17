using System;
using System.Collections.Generic;

namespace VisionNet.Core.Threading
{
    public class TransactionalAction: IDisposable
    {
        private readonly object _lockObject;
        private static readonly Dictionary<string, object> _lockObjects = new Dictionary<string, object>();
        private static readonly object _lockCreationObject = new object();
        private readonly string _id;
        private bool disposed = false;
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

        public void Execute(Action action)
        {
            lock (_lockObject)
            {
                action.Invoke();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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

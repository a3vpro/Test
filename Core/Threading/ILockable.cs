using System;

namespace VisionNet.Core.Threading
{
    public interface ILockable
    {
        TransactionalAction Transactional { get; }

        void ExecuteInExclusiveLock(Action action);
    }
}

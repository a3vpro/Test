namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Represents an abstract class that links two objects of different types and manages their link status.
    /// </summary>
    /// <typeparam name="T1">The type of the first target object.</typeparam>
    /// <typeparam name="T2">The type of the second target object.</typeparam>
    public abstract class Linker<T1, T2>
    {
        /// <summary>
        /// The first target object to be linked.
        /// </summary>
        protected T1 _target1;

        /// <summary>
        /// The second target object to be linked.
        /// </summary>
        protected T2 _target2;

        /// <summary>
        /// Gets or sets the current link status.
        /// </summary>
        /// <value>
        /// The current link status, represented as an instance of <see cref="LinkStatus"/>.
        /// </value>
        public LinkStatus Status { get; protected set; } = LinkStatus.Unlinked;

        /// <summary>
        /// Initializes a new instance of the <see cref="Linker{T1, T2}"/> class with specified target objects.
        /// </summary>
        /// <param name="target1">The first target object to be linked.</param>
        /// <param name="target2">The second target object to be linked.</param>
        public Linker(T1 target1, T2 target2)
        {
            _target1 = target1;
            _target2 = target2;
        }

        /// <summary>
        /// Links the two target objects if they are currently unlinked.
        /// Changes the link status to <see cref="LinkStatus.Linked"/> if successful.
        /// </summary>
        public void Link()
        {
            if (Status == LinkStatus.Unlinked)
            {
                var success = _tryLink();
                if (success)
                    Status = LinkStatus.Linked;
            }
        }

        /// <summary>
        /// Unlinks the two target objects if they are currently linked.
        /// Changes the link status to <see cref="LinkStatus.Unlinked"/> if successful.
        /// </summary>
        public void Unlink()
        {
            if (Status == LinkStatus.Linked)
            {
                var success = _tryUnlink();
                if (success)
                    Status = LinkStatus.Unlinked;
            }
        }

        /// <summary>
        /// Attempts to link the two target objects.
        /// This method must be implemented by a derived class.
        /// </summary>
        /// <returns><c>true</c> if the linking was successful, otherwise <c>false</c>.</returns>
        protected abstract bool _tryLink();

        /// <summary>
        /// Attempts to unlink the two target objects.
        /// This method must be implemented by a derived class.
        /// </summary>
        /// <returns><c>true</c> if the unlinking was successful, otherwise <c>false</c>.</returns>
        protected abstract bool _tryUnlink();
    }

}

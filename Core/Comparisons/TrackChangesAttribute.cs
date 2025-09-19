using System;

namespace VisionNet.Core.Comparisons
{
    /// <summary>
    /// Specifies whether a property participates in change tracking performed by <c>ChangeTracker</c>.
    /// All properties are tracked unless this attribute explicitly opts them out.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class TrackChangesAttribute : Attribute
    {
        /// <summary>
        /// Gets a value indicating whether the decorated property should be included in change tracking.
        /// </summary>
        public bool Track { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackChangesAttribute"/> class with the desired tracking behavior.
        /// </summary>
        /// <param name="track">Set to <see langword="true"/> to include the property in change tracking, or <see langword="false"/> to exclude it. Defaults to <see langword="true"/>.</param>
        public TrackChangesAttribute(bool track = true)
        {
            Track = track;
        }
    }
}

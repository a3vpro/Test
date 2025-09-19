using Force.DeepCloner;

namespace VisionNet.Core.Cloning
{
    public static class CloneExtensions
    {
        /// <summary>
        /// Creates a clone of the <paramref name="source"/> instance using either a shallow or deep copy operation.
        /// </summary>
        /// <typeparam name="T">
        /// A reference or value type supported by the Force.DeepCloner library for cloning.
        /// </typeparam>
        /// <param name="source">
        /// The instance to clone. When <see langword="null"/>, the method returns the default value of
        /// <typeparamref name="T"/>.
        /// </param>
        /// <param name="deepCopy">
        /// <see langword="true"/> to perform a deep clone of the entire object graph; otherwise, a shallow clone is created.
        /// </param>
        /// <returns>
        /// The cloned instance, or the default value of <typeparamref name="T"/> when <paramref name="source"/> is
        /// <see langword="null"/>.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Thrown when the underlying cloning library cannot clone the specified type or when the object graph contains unsupported members.</exception>
        public static T Clone<T>(this T source, bool deepCopy = false)
        {
            if (source == null) return default;

            return deepCopy
                ? source.DeepClone()
                : source.ShallowClone();
        }
    }
}

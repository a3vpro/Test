namespace VisionNet.Core.IOC
{
    /// <summary>
    /// Provides the base contract for resolving a service descriptor into a registration key that can be
    /// consumed by the dependency injection container.
    /// Implementations convert metadata of type <typeparamref name="T"/> into a unique string identifier.
    /// </summary>
    /// <typeparam name="T">The metadata type used to describe the service being resolved.</typeparam>
    public abstract class ClassKeyResolver<T>
    {
        protected abstract string ClassType { get; }

        /// <summary>
        /// Resolves the container registration key for the supplied service description.
        /// Implementations must combine the provided metadata and optional class name into a stable, unique key
        /// that matches the registration stored under <see cref="ClassType"/>.
        /// </summary>
        /// <param name="type">The service metadata or type information to translate into a registration key. Must not be <see langword="null"/> for reference types.</param>
        /// <param name="className">An optional explicit class name to disambiguate implementations when multiple classes share the same <paramref name="type"/>. Use <see cref="string.Empty"/> to rely solely on <paramref name="type"/>.</param>
        /// <returns>A non-empty registration key that uniquely identifies the service within the container.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/> for reference-type metadata.</exception>
        /// <exception cref="ArgumentException">Thrown when the combination of <paramref name="type"/> and <paramref name="className"/> cannot be mapped to a valid registration key.</exception>
        public abstract string Resolve(T type, string className = "");

        //public static string Resolve(T type, string className = "")
        //{
        //    return InternalResolve(type, className);
        //}
    }
}

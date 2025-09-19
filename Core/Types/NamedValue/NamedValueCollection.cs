using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VisionNet.Core.Types
{
    /// <summary>
    /// Provides a thread-safe, in-memory collection of <see cref="NamedValue"/> instances keyed by their identifier.
    /// </summary>
    /// <remarks>
    /// The collection preserves no ordering guarantees and produces snapshot copies when enumerating all values.
    /// </remarks>
    public class NamedValueCollection : INamedValueCollection
    {
        private readonly ConcurrentDictionary<string, NamedValue> _values = new ConcurrentDictionary<string, NamedValue>();

        /// <summary>
        /// Gets the total number of named values currently stored in the collection.
        /// </summary>
        /// <returns>The count of entries that are accessible via their identifiers.</returns>
        public int Count()
        {
            return _values.Count;
        }

        /// <summary>
        /// Removes the <see cref="NamedValue"/> associated with the specified identifier, if it exists.
        /// </summary>
        /// <param name="id">The unique identifier used to locate the stored value. Cannot be <c>null</c>.</param>
        /// <remarks>The method completes silently when the identifier does not exist in the collection.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is <c>null</c>.</exception>
        public void Delete(string id)
        {
            _values.TryRemove(id, out var value);
        }

        /// <summary>
        /// Determines whether a <see cref="NamedValue"/> is present for the specified identifier.
        /// </summary>
        /// <param name="id">The identifier to test for membership. Cannot be <c>null</c>.</param>
        /// <returns><c>true</c> if a value exists for the identifier; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is <c>null</c>.</exception>
        public bool Exists(string id)
        {
            return _values.ContainsKey(id);
        }

        /// <summary>
        /// Retrieves the <see cref="NamedValue"/> associated with the given identifier, if it is present.
        /// </summary>
        /// <param name="id">The identifier whose associated value is requested. Cannot be <c>null</c>.</param>
        /// <returns>The located <see cref="NamedValue"/>, or <c>null</c> when the identifier is not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is <c>null</c>.</exception>
        public NamedValue Get(string id)
        {
            _values.TryGetValue(id, out var entity);
            return entity;
        }

        /// <summary>
        /// Retrieves a snapshot list of all <see cref="NamedValue"/> entries stored in the collection.
        /// </summary>
        /// <returns>An <see cref="IList{T}"/> containing copies of the values at the time of invocation.</returns>
        public IList<NamedValue> GetAll()
        {
            return _values.Values.ToList();
        }

        /// <summary>
        /// Adds a new <see cref="NamedValue"/> to the collection using the provided identifier.
        /// </summary>
        /// <param name="id">The unique identifier that will index the stored value. Cannot be <c>null</c>.</param>
        /// <param name="entity">The <see cref="NamedValue"/> instance to store. Cannot be <c>null</c>.</param>
        /// <remarks>If an entry with the same identifier already exists, the operation fails silently and the existing value is preserved.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> or <paramref name="entity"/> is <c>null</c>.</exception>
        public void Insert(string id, NamedValue entity)
        {
            _values.TryAdd(id, entity);
        }

        /// <summary>
        /// Updates the <see cref="NamedValue"/> stored for the specified identifier using the provided transformation.
        /// </summary>
        /// <param name="id">The identifier whose associated value should be updated. Cannot be <c>null</c>.</param>
        /// <param name="updateAction">A delegate that produces the new value based on the current value. Cannot be <c>null</c>.</param>
        /// <returns>The updated <see cref="NamedValue"/>, or <c>null</c> if the identifier did not previously exist.</returns>
        /// <remarks>
        /// The update is applied atomically per key. If the key is missing, no update occurs and the method returns <c>null</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="id"/> or <paramref name="updateAction"/> is <c>null</c>.
        /// </exception>
        public NamedValue Update(string id, Func<NamedValue, NamedValue> updateAction)
        {
            NamedValue result = default(NamedValue);
            var success = _values.TryGetValue(id, out result);
            if (success)
            {
                result = updateAction(result);
                _values.AddOrUpdate(id, result, (i, e) => e);
            }
            return result;
        }
    }
}

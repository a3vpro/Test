using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
#if (NET472 || NET481 || NETSTANDARD2_0)
using System.Diagnostics.CodeAnalysis;
#endif

namespace VisionNet.Core.Collections
{
    /// <summary>
    /// Represents a structure that holds an array of objects.
    /// </summary>
    public struct ArrayTuple
    {
        private object[] _array;

        /// <summary>
        /// Creates an <see cref="ArrayTuple"/> instance from a collection of collapsed keys and additional keys.
        /// </summary>
        /// <param name="collapsedKeys">A list of tuples containing an object, a set of integers, and an integer representing the collapsed keys.</param>
        /// <param name="keys">Additional keys to be included in the array.</param>
        /// <returns>An <see cref="ArrayTuple"/> instance containing the combined keys.</returns>
        public static ArrayTuple From(IList<(object, ISet<int>, int)> collapsedKeys, params object[] keys)
        {
            var array = new object[collapsedKeys.Count + keys.Length];
            for (var i = collapsedKeys.Count - 1; i >= 0; i--)
            {
                for (var j = array.Length - 1; j > collapsedKeys[i].Item3; j--)
                {
                    array[j] = array[j - 1];
                }
                array[collapsedKeys[i].Item3] = collapsedKeys[i].Item1;
            }
            for (int i = 0, j = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                    array[i] = keys[j++];
            }
            return new ArrayTuple()
            {
                _array = array
            };
        }

        /// <summary>
        /// Creates an <see cref="ArrayTuple"/> instance from a list of keys.
        /// </summary>
        /// <param name="keys">The keys to be included in the array.</param>
        /// <returns>An <see cref="ArrayTuple"/> instance containing the provided keys.</returns>
        public static ArrayTuple From(params object[] keys)
        {
            return new ArrayTuple()
            {
                _array = keys
            };
        }

#if (NET472 || NET481 || NETSTANDARD2_0)
        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="ArrayTuple"/> instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified object is equal to the current instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
#else
        /// <inheritdoc cref="Equals(object)"/>
        public override bool Equals([NotNullWhen(true)] object? obj)
#endif
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null) return false;
            if (!(obj is ArrayTuple tup)) return false;
            if (_array.Length != tup._array.Length) return false;
            return _array.SequenceEqual(tup._array);
        }

        /// <summary>
        /// Computes the hash code for the current <see cref="ArrayTuple"/> instance.
        /// </summary>
        /// <returns>The hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = _array[0].GetHashCode();
                for (var i = 1; i < _array.Length; i++)
                {
                    hash = hash * 23 + _array[i].GetHashCode();
                }
                return hash;
            }
        }
    }

    /// <summary>
    /// Represents a dictionary that allows for multiple keys of different types (T1, T2, T3, T4, T5) to be used for indexing.
    /// </summary>
    /// <typeparam name="T1">The type of the first key.</typeparam>
    /// <typeparam name="T2">The type of the second key.</typeparam>
    /// <typeparam name="T3">The type of the third key.</typeparam>
    /// <typeparam name="T4">The type of the fourth key.</typeparam>
    /// <typeparam name="T5">The type of the fifth key.</typeparam>
    /// <typeparam name="TValue">The type of the values stored in the dictionary.</typeparam>
    public class MultiKeyDictionary<T1, T2, T3, T4, T5, TValue> : IEnumerable<TValue>
#if !(NET472 || NET481 || NETSTANDARD2_0)
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
    where T4 : notnull
    where T5 : notnull
#endif
    {
        private IDictionary<T1, ISet<int>> _indices1;
        private IDictionary<T2, ISet<int>> _indices2;
        private IDictionary<T3, ISet<int>> _indices3;
        private IDictionary<T4, ISet<int>> _indices4;
        private IDictionary<T5, ISet<int>> _indices5;
        private IDictionary<ArrayTuple, int> _fullIndex;
        private IList<TValue> _data;
        private IList<T1> _keys1;
        private IList<T2> _keys2;
        private IList<T3> _keys3;
        private IList<T4> _keys4;
        private IList<T5> _keys5;
        private IList<(object, ISet<int>, int)> _collapsedKeys;
        private ISet<int> _unusedIndices;

        /// <summary>
        /// Removals do not actually remove data from the table, but just delete the index.
        /// If a lot of Remove operations are performed, the table will become very sparsely populated with actual data.
        /// This setting controls the absolute number of unused indices that will trigger a compaction.
        /// Both this and the relative threshold must be met for a compaction to occur.
        /// </summary>
        /// <value>The number of unused indices before a compaction is performed.</value>
        public int CompactingAbsoluteThreshold { get; set; } = 1000;
        /// <summary>
        /// Removals do not actually remove data from the table, but just delete the index.
        /// If a lot of Remove operations are performed, the table will become very sparsely populated with actual data.
        /// This setting controls the relative number of unused indices that will trigger a compaction.
        /// Both this and the absolute threshold must be met for a compaction to occur.
        /// </summary>
        /// <value>The relative number of unused indices (relative to the full size of the array) before a compaction is performed.</value>
        public double CompactingRelativeThreshold { get; set; } = 0.01;

        protected internal MultiKeyDictionary(IDictionary<T1, ISet<int>> indices1,
            IDictionary<T2, ISet<int>> indices2, IDictionary<T3, ISet<int>> indices3,
            IDictionary<T4, ISet<int>> indices4, IDictionary<T5, ISet<int>> indices5,
            IDictionary<ArrayTuple, int> fullIndex, IList<TValue> data,
            IList<T1> keys1, IList<T2> keys2, IList<T3> keys3, IList<T4> keys4, IList<T5> keys5,
            IList<(object, ISet<int>, int)> collapsedKeys, ISet<int> unusedIndices)
        {
            _indices1 = indices1;
            _indices2 = indices2;
            _indices3 = indices3;
            _indices4 = indices4;
            _indices5 = indices5;
            _fullIndex = fullIndex;
            _data = data;
            _keys1 = keys1;
            _keys2 = keys2;
            _keys3 = keys3;
            _keys4 = keys4;
            _keys5 = keys5;
            _collapsedKeys = collapsedKeys;
            _unusedIndices = unusedIndices;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyDictionary{T1, T2, T3, T4, T5, TValue}"/> class with no data.
        /// </summary>
        public MultiKeyDictionary() : this(new KeyValuePair<(T1, T2, T3, T4, T5), TValue>[0]) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyDictionary{T1, T2, T3, T4, T5, TValue}"/> class with the specified collection of key-value pairs.
        /// </summary>
        /// <param name="data">An enumerable collection of key-value pairs, where the key is a tuple of five elements (T1, T2, T3, T4, T5) and the value is of type <typeparamref name="TValue"/>.</param>
        public MultiKeyDictionary(IEnumerable<KeyValuePair<(T1, T2, T3, T4, T5), TValue>> data)
            : this(data.Select(x => (x.Key.Item1, x.Key.Item2, x.Key.Item3, x.Key.Item4, x.Key.Item5, x.Value))) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyDictionary{T1, T2, T3, T4, T5, TValue}"/> class with the specified collection of tuples.
        /// </summary>
        /// <param name="data">An enumerable collection of tuples, where each tuple contains five keys of types <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, and <typeparamref name="T5"/>, followed by a value of type <typeparamref name="TValue"/>.</param>
        public MultiKeyDictionary(IEnumerable<(T1, T2, T3, T4, T5, TValue)> data)
        {
            _indices1 = new Dictionary<T1, ISet<int>>();
            _indices2 = new Dictionary<T2, ISet<int>>();
            _indices3 = new Dictionary<T3, ISet<int>>();
            _indices4 = new Dictionary<T4, ISet<int>>();
            _indices5 = new Dictionary<T5, ISet<int>>();
            _fullIndex = new Dictionary<ArrayTuple, int>(100);
            _data = new List<TValue>(100);
            _keys1 = new List<T1>(100);
            _keys2 = new List<T2>(100);
            _keys3 = new List<T3>(100);
            _keys4 = new List<T4>(100);
            _keys5 = new List<T5>(100);
            _collapsedKeys = new List<(object, ISet<int>, int)>();
            _unusedIndices = new HashSet<int>();

            var idx = -1;
            foreach (var d in data)
            {
                ++idx;
                var (key1, key2, key3, key4, key5, val) = d;
                _data.Add(val);
                _fullIndex[ArrayTuple.From(key1, key2, key3, key4, key5)] = idx;
                _keys1.Add(key1);
                _keys2.Add(key2);
                _keys3.Add(key3);
                _keys4.Add(key4);
                _keys5.Add(key5);
                if (!_indices1.TryGetValue(key1, out var ind1))
                {
                    ind1 = new HashSet<int>();
                    _indices1[key1] = ind1;
                }
                ind1.Add(idx);
                if (!_indices2.TryGetValue(key2, out var ind2))
                {
                    ind2 = new HashSet<int>();
                    _indices2[key2] = ind2;
                }
                ind2.Add(idx);
                if (!_indices3.TryGetValue(key3, out var ind3))
                {
                    ind3 = new HashSet<int>();
                    _indices3[key3] = ind3;
                }
                ind3.Add(idx);
                if (!_indices4.TryGetValue(key4, out var ind4))
                {
                    ind4 = new HashSet<int>();
                    _indices4[key4] = ind4;
                }
                ind4.Add(idx);
                if (!_indices5.TryGetValue(key5, out var ind5))
                {
                    ind5 = new HashSet<int>();
                    _indices5[key5] = ind5;
                }
                ind5.Add(idx);
            }
        }

        /// <summary>
        /// Gets the number of elements in the dictionary, accounting for unused indices if applicable.
        /// </summary>
        /// <returns>The number of elements in the dictionary.</returns>
        /// <summary>
        /// Gets the number of stored values, accounting for unused indices and any active slice.
        /// </summary>
        /// <returns>The number of accessible entries in the dictionary or slice.</returns>
        /// <summary>
        /// Gets the number of stored values, accounting for unused indices and any active slice.
        /// </summary>
        /// <returns>The number of accessible entries in the dictionary or slice.</returns>
        /// <summary>
        /// Gets the number of stored values, accounting for unused indices and any active slice.
        /// </summary>
        /// <returns>The number of accessible entries in the dictionary or slice.</returns>
        /// <summary>
        /// Gets the number of stored values, accounting for unused indices and any active slice.
        /// </summary>
        /// <returns>The number of accessible entries in the dictionary or slice.</returns>
        public int Count => GetCollapsedIndexsetOrDefault()?.Count ?? (_data.Count - _unusedIndices.Count);


        /// <summary>
        /// Sets *all* values in the table (or the slice) to the given value.
        /// If a collapsed index set is defined, only the values at those indices are updated.
        /// Otherwise, all values in the table are set to the given value.
        /// </summary>
        /// <param name="value">The value to set for all elements in the table (or slice).</param>
        public void Set(TValue value)
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices == null)
            {
                // No collapsed indices, so set all values in the data list
                for (var i = 0; i < _data.Count; i++)
                {
                    // This method also overrides unused indices
                    _data[i] = value;
                }
            }
            else
            {
                // A collapsed index set is provided, so only set the values at those indices
                foreach (var index in collapsedIndices)
                    _data[index] = value;
            }
        }

        /// <summary>
        /// Removes the entry with the specified keys from the dictionary.
        /// </summary>
        /// <param name="key1">The first key of type <typeparamref name="T1"/>.</param>
        /// <param name="key2">The second key of type <typeparamref name="T2"/>.</param>
        /// <param name="key3">The third key of type <typeparamref name="T3"/>.</param>
        /// <param name="key4">The fourth key of type <typeparamref name="T4"/>.</param>
        /// <param name="key5">The fifth key of type <typeparamref name="T5"/>.</param>
        /// <returns><c>true</c> if the entry was successfully removed; otherwise, <c>false</c>.</returns>
        public bool Remove(T1 key1, T2 key2, T3 key3, T4 key4, T5 key5)

        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            ExceptionHandling.ThrowIfNull(key4, nameof(key4));
            ExceptionHandling.ThrowIfNull(key5, nameof(key5));

            var key = ArrayTuple.From(_collapsedKeys, key1, key2, key3, key4, key5);
            if (!_fullIndex.TryGetValue(key, out var index))
            {
                return false;
            }
            _fullIndex.Remove(key);
            _unusedIndices.Add(index);
            _data[index] = default;
            _keys1[index] = default;
            _keys2[index] = default;
            _keys3[index] = default;
            _keys4[index] = default;
            _keys5[index] = default;
            for (var i = 0; i < _collapsedKeys.Count; i++)
            {
                _collapsedKeys[i].Item2.Remove(index); // we don't need to manipulate all indices > index, because we don't remove above
            }
            _indices1[key1].Remove(index);
            _indices2[key2].Remove(index);
            _indices3[key3].Remove(index);
            _indices4[key4].Remove(index);
            _indices5[key5].Remove(index);
            CheckCompact();
            return true;
        }

        /// <summary>
        /// Removes all stored key combinations and values from the dictionary or the currently active slice.
        /// </summary>
        /// <remarks>
        /// When the dictionary represents a slice generated by collapsing keys, only entries that participate in that
        /// slice are removed while index structures are updated accordingly.
        /// </remarks>
        public void Clear()
        {
            var collapsed = GetCollapsedIndexsetOrDefault();
            if (collapsed == null)
            {
                _fullIndex.Clear();
                _data.Clear();
                _keys1.Clear();
                _keys2.Clear();
                _keys3.Clear();
                _keys4.Clear();
                _keys5.Clear();
                _unusedIndices.Clear();
                foreach (var indices in _indices1.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _indices2.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _indices3.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _indices4.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _indices5.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _collapsedKeys)
                {
                    indices.Item2.Clear();
                }
            }
            else
            {
                foreach (var ind in collapsed)
                {
                    var key = ArrayTuple.From(_collapsedKeys, _keys1[ind], _keys2[ind], _keys3[ind], _keys4[ind], _keys5[ind]);
                    _fullIndex.Remove(key);
                }
                foreach (var indices in _indices1.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _indices2.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _indices3.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _indices4.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _indices5.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _collapsedKeys)
                {
                    indices.Item2.ExceptWith(collapsed);
                }
                // declare the indices unused and then compact
                _unusedIndices.UnionWith(collapsed);
                Compact();
            }
        }
        
        protected void CheckCompact()
        {
            if (_unusedIndices.Count > CompactingAbsoluteThreshold
                && _unusedIndices.Count > _data.Count * CompactingRelativeThreshold)
                Compact();
        }

        private void Compact()
        {
            if (_unusedIndices.Count == 0) return;
            var sequence = _unusedIndices.OrderByDescending(x => x).ToList();
            var offsets = new Dictionary<int, int>();
            var offset = 0;
            var offsetIndex = 0;
            for (var i = sequence.Count - 1; i >= 0; i--)
            {
                var index = sequence[i];
                if (offset > 0)
                {
                    for (var o = offsetIndex; o < index; o++)
                    {
                        offsets[o] = offset;
                    }
                }
                offsetIndex = index;
                offset++;
                _data.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys1.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys2.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys3.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys4.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys5.RemoveAt(sequence[sequence.Count - 1 - i]);
            }
            for (var o = offsetIndex; o < _data.Count + offset; o++)
            {
                offsets[o] = offset;
            }
            foreach (var kvp in _fullIndex.ToArray())
            {
                if (offsets.TryGetValue(kvp.Value, out var off))
                {
                    _fullIndex[kvp.Key] = kvp.Value - off;
                }
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(_unusedIndices);
                var reindex = indices.Item2.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.Item2.ExceptWith(reindex.Select(x => x.index));
                    indices.Item2.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices1)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices2)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices3)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices4)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices5)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            _unusedIndices.Clear();
        }

        /// <summary>
        /// Determines whether a value exists for the specified five-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="key4">The fourth component of the composite key.</param>
        /// <param name="key5">The fifth component of the composite key.</param>
        /// <returns><c>true</c> if the composite key currently references a stored value; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key parameter is <c>null</c>.</exception>
        public bool Contains(T1 key1, T2 key2, T3 key3, T4 key4, T5 key5)
        {
            return _fullIndex.ContainsKey(ArrayTuple.From(_collapsedKeys, key1, key2, key3, key4, key5));
        }

#if NET472 || NET481 || NETSTANDARD2_0
        /// <summary>
        /// Retrieves the collapsed index set when the dictionary operates on a sliced view of its data.
        /// </summary>
        /// <returns>The set of indices representing the active slice, or <c>null</c> when no slice is active.</returns>
        protected ISet<int> GetCollapsedIndexsetOrDefault()
        {
            HashSet<int> collapsedIndices = null;
#else
        /// <summary>
        /// Retrieves the collapsed index set when the dictionary operates on a sliced view of its data.
        /// </summary>
        /// <returns>The set of indices representing the active slice, or <c>null</c> when no slice is active.</returns>
        protected ISet<int>? GetCollapsedIndexsetOrDefault()
        {
            HashSet<int>? collapsedIndices = null;
#endif
            if (_collapsedKeys.Count > 0)
            {
                foreach (var collapsedSet in _collapsedKeys.Select(x => x.Item2).OrderBy(x => x.Count))
                {
                    if (collapsedIndices == null)
                        collapsedIndices = new HashSet<int>(collapsedSet);
                    else collapsedIndices.IntersectWith(collapsedSet);
                }
            }
            return collapsedIndices;
        }

        /// <summary>
        /// Enumerates values currently visible through the dictionary, including respect for collapsed slices.
        /// </summary>
        /// <returns>An iterator that yields each value stored under the applicable key combinations.</returns>
        /// <summary>
        /// Enumerates values currently visible within the dictionary, respecting any active slice.
        /// </summary>
        /// <returns>An iterator that yields values for each stored key combination in scope.</returns>
        /// <summary>
        /// Enumerates values currently visible within the dictionary, respecting any active slice.
        /// </summary>
        /// <returns>An iterator that yields values for each stored key combination in scope.</returns>
        /// <summary>
        /// Enumerates values currently visible within the dictionary, respecting any active slice.
        /// </summary>
        /// <returns>An iterator that yields values for each stored key within scope.</returns>
        public IEnumerable<TValue> EnumerateValues()
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices == null)
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (!_unusedIndices.Contains(i))
                        yield return _data[i];
                }
            }
            else
            {
                foreach (var index in collapsedIndices)
                {
                    yield return _data[index];
                }
            }
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return EnumerateValues().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumerateValues().GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the value associated with the specified five-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="key4">The fourth component of the composite key.</param>
        /// <param name="key5">The fifth component of the composite key.</param>
        /// <returns>The value stored under the specified composite key.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when attempting to retrieve a value for a key combination that has not been stored.</exception>
        public TValue this[T1 key1, T2 key2, T3 key3, T4 key4, T5 key5]
        {
            get
            {
                ExceptionHandling.ThrowIfNull(key1, nameof(key1));
                ExceptionHandling.ThrowIfNull(key2, nameof(key2));
                ExceptionHandling.ThrowIfNull(key3, nameof(key3));
                ExceptionHandling.ThrowIfNull(key4, nameof(key4));
                ExceptionHandling.ThrowIfNull(key5, nameof(key5));
                if (!_fullIndex.TryGetValue(ArrayTuple.From(_collapsedKeys, key1, key2, key3, key4, key5), out var index))
                {
                    throw new ArgumentException($"Key combination {(key1, key2, key3, key4, key5)} not found");
                }

                return _data[index];
            }
            set
            {
                ExceptionHandling.ThrowIfNull(key1, nameof(key1));
                ExceptionHandling.ThrowIfNull(key2, nameof(key2));
                ExceptionHandling.ThrowIfNull(key3, nameof(key3));
                ExceptionHandling.ThrowIfNull(key4, nameof(key4));
                ExceptionHandling.ThrowIfNull(key5, nameof(key5));

                var tupKey = ArrayTuple.From(_collapsedKeys, key1, key2, key3, key4, key5);
                if (!_fullIndex.TryGetValue(tupKey, out var index))
                    index = -1;
                if (index < 0)
                {
                    if (!_indices1.TryGetValue(key1, out var indices1))
                    {
                        indices1 = new HashSet<int>();
                        _indices1[key1] = indices1;
                    }
                    if (!_indices2.TryGetValue(key2, out var indices2))
                    {
                        indices2 = new HashSet<int>();
                        _indices2[key2] = indices2;
                    }
                    if (!_indices3.TryGetValue(key3, out var indices3))
                    {
                        indices3 = new HashSet<int>();
                        _indices3[key3] = indices3;
                    }
                    if (!_indices4.TryGetValue(key4, out var indices4))
                    {
                        indices4 = new HashSet<int>();
                        _indices4[key4] = indices4;
                    }
                    if (!_indices5.TryGetValue(key5, out var indices5))
                    {
                        indices5 = new HashSet<int>();
                        _indices5[key5] = indices5;
                    }
                    if (_unusedIndices.Count > 0)
                    {
                        // reuse an unused index
                        index = _unusedIndices.First();
                        _unusedIndices.Remove(index);
                        _data[index] = value;
                        _keys1[index] = key1;
                        _keys2[index] = key2;
                        _keys3[index] = key3;
                        _keys4[index] = key4;
                        _keys5[index] = key5;
                    }
                    else
                    {
                        // make an insert at the end
                        index = _data.Count;
                        _data.Add(value);
                        _keys1.Add(key1);
                        _keys2.Add(key2);
                        _keys3.Add(key3);
                        _keys4.Add(key4);
                        _keys5.Add(key5);
                    }
                    _fullIndex[tupKey] = index;
                    indices1.Add(index);
                    indices2.Add(index);
                    indices3.Add(index);
                    indices4.Add(index);
                    indices5.Add(index);
                    // the data needs to be added to all collapsed dimensions
                    foreach (var f in _collapsedKeys)
                    {
                        f.Item2.Add(index);
                    }
                }
                else
                {
                    _data[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified tuple of key components.
        /// </summary>
        /// <param name="keys">A tuple describing the five components of the composite key.</param>
        /// <returns>The value stored under the provided tuple of keys.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any element of <paramref name="keys"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when attempting to retrieve a value for a key combination that has not been stored.</exception>
        public TValue this[(T1 key1, T2 key2, T3 key3, T4 key4, T5 key5) keys]
        {
            get => this[keys.key1, keys.key2, keys.key3, keys.key4, keys.key5];
            set => this[keys.key1, keys.key2, keys.key3, keys.key4, keys.key5] = value;
        }

        /// <summary>
        /// Adds a value associated with the specified five-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="key4">The fourth component of the composite key.</param>
        /// <param name="key5">The fifth component of the composite key.</param>
        /// <param name="value">The value to associate with the provided composite key.</param>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a value already exists for the provided key combination.</exception>
        public void Add(T1 key1, T2 key2, T3 key3, T4 key4, T5 key5, TValue value)
        {
            if (Contains(key1, key2, key3, key4, key5)) throw new ArgumentException($"Key already exists ({key1}, {key2}, {key3}, {key4}, {key5})");
            this[key1, key2, key3, key4, key5] = value;
        }

        /// <summary>
        /// Adds a value associated with the specified tuple of key components.
        /// </summary>
        /// <param name="keys">A tuple containing the five key components.</param>
        /// <param name="value">The value to associate with the provided composite key.</param>
        /// <exception cref="ArgumentNullException">Thrown when any element of <paramref name="keys"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a value already exists for the provided key combination.</exception>
        public void Add((T1 key1, T2 key2, T3 key3, T4 key4, T5 key5) keys, TValue value) => Add(keys.key1, keys.key2, keys.key3, keys.key4, keys.key5, value);

        /// <summary>
        /// Removes all entries that use the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove1(T1 key1)
        {
            if (!_indices1.TryGetValue(key1, out var indices1) || indices1.Count == 0)
                return 0;
            foreach (var index in indices1)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index], _keys4[index], _keys5[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices1);
            }
            foreach (var indices in _indices2.Values)
            {
                indices.ExceptWith(indices1);
            }
            foreach (var indices in _indices3.Values)
            {
                indices.ExceptWith(indices1);
            }
            foreach (var indices in _indices4.Values)
            {
                indices.ExceptWith(indices1);
            }
            foreach (var indices in _indices5.Values)
            {
                indices.ExceptWith(indices1);
            }
            var removed = indices1.Count;
            _unusedIndices.UnionWith(indices1);
            indices1.Clear();
            CheckCompact();
            return removed;
        }

        /// <summary>
        /// Removes all entries that use the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove2(T2 key2)
        {
            if (!_indices2.TryGetValue(key2, out var indices2) || indices2.Count == 0)
                return 0;
            foreach (var index in indices2)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index], _keys4[index], _keys5[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices2);
            }
            foreach (var indices in _indices1.Values)
            {
                indices.ExceptWith(indices2);
            }
            foreach (var indices in _indices3.Values)
            {
                indices.ExceptWith(indices2);
            }
            foreach (var indices in _indices4.Values)
            {
                indices.ExceptWith(indices2);
            }
            foreach (var indices in _indices5.Values)
            {
                indices.ExceptWith(indices2);
            }
            var removed = indices2.Count;
            _unusedIndices.UnionWith(indices2);
            indices2.Clear();
            CheckCompact();
            return removed;
        }

        /// <summary>
        /// Removes all entries that use the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove3(T3 key3)
        {
            if (!_indices3.TryGetValue(key3, out var indices3) || indices3.Count == 0)
                return 0;
            foreach (var index in indices3)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index], _keys4[index], _keys5[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices3);
            }
            foreach (var indices in _indices1.Values)
            {
                indices.ExceptWith(indices3);
            }
            foreach (var indices in _indices2.Values)
            {
                indices.ExceptWith(indices3);
            }
            foreach (var indices in _indices4.Values)
            {
                indices.ExceptWith(indices3);
            }
            foreach (var indices in _indices5.Values)
            {
                indices.ExceptWith(indices3);
            }
            var removed = indices3.Count;
            _unusedIndices.UnionWith(indices3);
            indices3.Clear();
            CheckCompact();
            return removed;
        }

        /// <summary>
        /// Removes all entries that use the specified fourth key component.
        /// </summary>
        /// <param name="key4">The fourth key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove4(T4 key4)
        {
            if (!_indices4.TryGetValue(key4, out var indices4) || indices4.Count == 0)
                return 0;
            foreach (var index in indices4)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index], _keys4[index], _keys5[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices4);
            }
            foreach (var indices in _indices1.Values)
            {
                indices.ExceptWith(indices4);
            }
            foreach (var indices in _indices2.Values)
            {
                indices.ExceptWith(indices4);
            }
            foreach (var indices in _indices3.Values)
            {
                indices.ExceptWith(indices4);
            }
            foreach (var indices in _indices5.Values)
            {
                indices.ExceptWith(indices4);
            }
            var removed = indices4.Count;
            _unusedIndices.UnionWith(indices4);
            indices4.Clear();
            CheckCompact();
            return removed;
        }

        /// <summary>
        /// Removes all entries that use the specified fifth key component.
        /// </summary>
        /// <param name="key5">The fifth key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove5(T5 key5)
        {
            if (!_indices5.TryGetValue(key5, out var indices5) || indices5.Count == 0)
                return 0;
            foreach (var index in indices5)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index], _keys4[index], _keys5[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices5);
            }
            foreach (var indices in _indices1.Values)
            {
                indices.ExceptWith(indices5);
            }
            foreach (var indices in _indices2.Values)
            {
                indices.ExceptWith(indices5);
            }
            foreach (var indices in _indices3.Values)
            {
                indices.ExceptWith(indices5);
            }
            foreach (var indices in _indices4.Values)
            {
                indices.ExceptWith(indices5);
            }
            var removed = indices5.Count;
            _unusedIndices.UnionWith(indices5);
            indices5.Clear();
            CheckCompact();
            return removed;
        }

#if NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0
        /// <summary>
        /// Attempts to retrieve the value associated with the specified five-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="key4">The fourth component of the composite key.</param>
        /// <param name="key5">The fifth component of the composite key.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key combination, if found; otherwise, the default value for <typeparamref name="TValue"/>.</param>
        /// <returns><c>true</c> when the key combination exists; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(T1 key1, T2 key2, T3 key3, T4 key4, T5 key5, out TValue value)
#else
        /// <inheritdoc cref="TryGetValue(T1, T2, T3, T4, T5, out TValue)"/>
        public bool TryGetValue(T1 key1, T2 key2, T3 key3, T4 key4, T5 key5, out TValue? value)
#endif
        {
            if (!_fullIndex.TryGetValue(ArrayTuple.From(_collapsedKeys, key1, key2, key3, key4, key5), out var index))
            {
                value = default;
                return false;
            }
            value = _data[index];
            return true;
        }

        /// <summary>
        /// Creates a four-dimensional slice scoped to the provided first key component.
        /// </summary>
        /// <param name="key1">The first key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T2, T3, T4, T5, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T2, T3, T4, T5, TValue> Slice1(T1 key1)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            var ind = _indices1[key1];
            if (ind.Count == 0) throw new ArgumentException($"key {key1} not found", nameof(key1));
            return new MultiKeyDictionary<T2, T3, T4, T5, TValue>(
                _indices2, _indices3, _indices4, _indices5,
                _fullIndex,
                _data, _keys2, _keys3, _keys4, _keys5,
                _collapsedKeys.Concat(new[] { ((object)key1, ind, 0) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Creates a four-dimensional slice scoped to the provided second key component.
        /// </summary>
        /// <param name="key2">The second key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T1, T3, T4, T5, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T1, T3, T4, T5, TValue> Slice2(T2 key2)
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            var ind = _indices2[key2];
            if (ind.Count == 0) throw new ArgumentException($"key {key2} not found", nameof(key2));
            return new MultiKeyDictionary<T1, T3, T4, T5, TValue>(
                _indices1, _indices3, _indices4, _indices5,
                _fullIndex,
                _data, _keys1, _keys3, _keys4, _keys5,
                _collapsedKeys.Concat(new[] { ((object)key2, ind, 1) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Creates a four-dimensional slice scoped to the provided third key component.
        /// </summary>
        /// <param name="key3">The third key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T1, T2, T4, T5, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T1, T2, T4, T5, TValue> Slice3(T3 key3)
        {
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            var ind = _indices3[key3];
            if (ind.Count == 0) throw new ArgumentException($"key {key3} not found", nameof(key3));
            return new MultiKeyDictionary<T1, T2, T4, T5, TValue>(
                _indices1, _indices2, _indices4, _indices5,
                _fullIndex,
                _data, _keys1, _keys2, _keys4, _keys5,
                _collapsedKeys.Concat(new[] { ((object)key3, ind, 2) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Creates a four-dimensional slice scoped to the provided fourth key component.
        /// </summary>
        /// <param name="key4">The fourth key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T1, T2, T3, T5, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key4"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T1, T2, T3, T5, TValue> Slice4(T4 key4)
        {
            ExceptionHandling.ThrowIfNull(key4, nameof(key4));
            var ind = _indices4[key4];
            if (ind.Count == 0) throw new ArgumentException($"key {key4} not found", nameof(key4));
            return new MultiKeyDictionary<T1, T2, T3, T5, TValue>(
                _indices1, _indices2, _indices3, _indices5,
                _fullIndex,
                _data, _keys1, _keys2, _keys3, _keys5,
                _collapsedKeys.Concat(new[] { ((object)key4, ind, 3) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Creates a four-dimensional slice scoped to the provided fifth key component.
        /// </summary>
        /// <param name="key5">The fifth key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T1, T2, T3, T4, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key5"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T1, T2, T3, T4, TValue> Slice5(T5 key5)
        {
            ExceptionHandling.ThrowIfNull(key5, nameof(key5));
            var ind = _indices5[key5];
            if (ind.Count == 0) throw new ArgumentException($"key {key5} not found", nameof(key5));
            return new MultiKeyDictionary<T1, T2, T3, T4, TValue>(
                _indices1, _indices2, _indices3, _indices4,
                _fullIndex,
                _data, _keys1, _keys2, _keys3, _keys4,
                _collapsedKeys.Concat(new[] { ((object)key5, ind, 4) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Determines whether any value exists that includes the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key1"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        /// <summary>
        /// Determines whether any value exists that includes the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key1"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        /// <summary>
        /// Determines whether any value exists that includes the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key1"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        /// <summary>
        /// Determines whether any value exists that includes the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key1"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        public bool ContainsKey1(T1 key1)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            if (!_indices1.TryGetValue(key1, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// Determines whether any value exists that includes the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key2"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        /// <summary>
        /// Determines whether any value exists that includes the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key2"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        /// <summary>
        /// Determines whether any value exists that includes the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key2"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        /// <summary>
        /// Determines whether any value exists that includes the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key2"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        public bool ContainsKey2(T2 key2)
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            if (!_indices2.TryGetValue(key2, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// Determines whether any value exists that includes the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key3"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        /// <summary>
        /// Determines whether any value exists that includes the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key3"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        /// <summary>
        /// Determines whether any value exists that includes the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key3"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        public bool ContainsKey3(T3 key3)
        {
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            if (!_indices3.TryGetValue(key3, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// Determines whether any value exists that includes the specified fourth key component.
        /// </summary>
        /// <param name="key4">The fourth key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key4"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key4"/> is <c>null</c>.</exception>
        /// <summary>
        /// Determines whether any value exists that includes the specified fourth key component.
        /// </summary>
        /// <param name="key4">The fourth key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key4"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key4"/> is <c>null</c>.</exception>
        public bool ContainsKey4(T4 key4)
        {
            ExceptionHandling.ThrowIfNull(key4, nameof(key4));
            if (!_indices4.TryGetValue(key4, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// Determines whether any value exists that includes the specified fifth key component.
        /// </summary>
        /// <param name="key5">The fifth key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key5"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key5"/> is <c>null</c>.</exception>
        public bool ContainsKey5(T5 key5)
        {
            ExceptionHandling.ThrowIfNull(key5, nameof(key5));
            if (!_indices5.TryGetValue(key5, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        public bool TrySlice1(T1 key1, out MultiKeyDictionary<T2, T3, T4, T5, TValue> values)
#else
        /// <inheritdoc cref="TrySlice1(T1, out MultiKeyDictionary{T2, T3, T4, T5, TValue})"/>
        public bool TrySlice1(T1 key1, out MultiKeyDictionary<T2, T3, T4, T5, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            if (_indices1.TryGetValue(key1, out var indices) && indices.Count > 0)
            {
                values = Slice1(key1);
                return true;
            }
            values = default;
            return false;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        public bool TrySlice2(T2 key2, out MultiKeyDictionary<T1, T3, T4, T5, TValue> values)
#else
        /// <inheritdoc cref="TrySlice2(T2, out MultiKeyDictionary{T1, T3, T4, T5, TValue})"/>
        public bool TrySlice2(T2 key2, out MultiKeyDictionary<T1, T3, T4, T5, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            if (_indices2.TryGetValue(key2, out var indices) && indices.Count > 0)
            {
                values = Slice2(key2);
                return true;
            }
            values = default;
            return false;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        public bool TrySlice3(T3 key3, out MultiKeyDictionary<T1, T2, T4, T5, TValue> values)
#else
        /// <inheritdoc cref="TrySlice3(T3, out MultiKeyDictionary{T1, T2, T4, T5, TValue})"/>
        public bool TrySlice3(T3 key3, out MultiKeyDictionary<T1, T2, T4, T5, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            if (_indices3.TryGetValue(key3, out var indices) && indices.Count > 0)
            {
                values = Slice3(key3);
                return true;
            }
            values = default;
            return false;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified fourth key component.
        /// </summary>
        /// <param name="key4">The fourth key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key4"/> is <c>null</c>.</exception>
        public bool TrySlice4(T4 key4, out MultiKeyDictionary<T1, T2, T3, T5, TValue> values)
#else
        /// <inheritdoc cref="TrySlice4(T4, out MultiKeyDictionary{T1, T2, T3, T5, TValue})"/>
        public bool TrySlice4(T4 key4, out MultiKeyDictionary<T1, T2, T3, T5, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key4, nameof(key4));
            if (_indices4.TryGetValue(key4, out var indices) && indices.Count > 0)
            {
                values = Slice4(key4);
                return true;
            }
            values = default;
            return false;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified fifth key component.
        /// </summary>
        /// <param name="key5">The fifth key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key5"/> is <c>null</c>.</exception>
        public bool TrySlice5(T5 key5, out MultiKeyDictionary<T1, T2, T3, T4, TValue> values)
#else
        /// <inheritdoc cref="TrySlice5(T5, out MultiKeyDictionary{T1, T2, T3, T4, TValue})"/>
        public bool TrySlice5(T5 key5, out MultiKeyDictionary<T1, T2, T3, T4, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key5, nameof(key5));
            if (_indices5.TryGetValue(key5, out var indices) && indices.Count > 0)
            {
                values = Slice5(key5);
                return true;
            }
            values = default;
            return false;
        }

        /// <summary>
        /// Enumerates the stored entries as tuples containing all key components and the associated value.
        /// </summary>
        /// <returns>An iterator that yields tuples of each composite key and its corresponding value.</returns>
        public IEnumerable<(T1, T2, T3, T4, T5, TValue)> Enumerate()
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices != null)
            {
                foreach (var i in collapsedIndices)
                {
                    yield return ((T1)_keys1[i], (T2)_keys2[i], (T3)_keys3[i], (T4)_keys4[i], (T5)_keys5[i], _data[i]);
                }
            }
            else
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (!_unusedIndices.Contains(i))
                        yield return ((T1)_keys1[i], (T2)_keys2[i], (T3)_keys3[i], (T4)_keys4[i], (T5)_keys5[i], _data[i]);
                }
            }
        }

        /// <summary>
        /// Materializes the dictionary into a standard <see cref="Dictionary{TKey, TValue}"/> using tuple-based composite keys.
        /// </summary>
        /// <returns>A dictionary containing every stored key combination and its associated value.</returns>
        public Dictionary<(T1, T2, T3, T4, T5), TValue> ToDictionary() => Enumerate().ToDictionary(x => (x.Item1, x.Item2, x.Item3, x.Item4, x.Item5), x => x.Item6);
    }

    /// <summary>
    /// Represents a dictionary that indexes values by four distinct key components with optional slicing support.
    /// </summary>
    /// <typeparam name="T1">The type of the first key component.</typeparam>
    /// <typeparam name="T2">The type of the second key component.</typeparam>
    /// <typeparam name="T3">The type of the third key component.</typeparam>
    /// <typeparam name="T4">The type of the fourth key component.</typeparam>
    /// <typeparam name="TValue">The type of values stored in the dictionary.</typeparam>
    public class MultiKeyDictionary<T1, T2, T3, T4, TValue> : IEnumerable<TValue>
#if !(NET472 || NET481 || NETSTANDARD2_0)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
#endif
    {
        private const int KEYS = 4;
        private IDictionary<T1, ISet<int>> _indices1;
        private IDictionary<T2, ISet<int>> _indices2;
        private IDictionary<T3, ISet<int>> _indices3;
        private IDictionary<T4, ISet<int>> _indices4;
        private IDictionary<ArrayTuple, int> _fullIndex;
        private IList<TValue> _data;
        private IList<T1> _keys1;
        private IList<T2> _keys2;
        private IList<T3> _keys3;
        private IList<T4> _keys4;
        private IList<(object, ISet<int>, int)> _collapsedKeys;
        private ISet<int> _unusedIndices;

        /// <summary>
        /// Removals do not actually remove data from the table, but just delete the index.
        /// If a lot of Remove operations are performed, the table will become very sparsely populated with actual data.
        /// This setting controls the absolute number of unused indices that will trigger a compaction.
        /// Both this and the relative threshold must be met for a compaction to occur.
        /// </summary>
        /// <value>The number of unused indices before a compaction is performed.</value>
        public int CompactingAbsoluteThreshold { get; set; } = 1000;
        /// <summary>
        /// Removals do not actually remove data from the table, but just delete the index.
        /// If a lot of Remove operations are performed, the table will become very sparsely populated with actual data.
        /// This setting controls the relative number of unused indices that will trigger a compaction.
        /// Both this and the absolute threshold must be met for a compaction to occur.
        /// </summary>
        /// <value>The relative number of unused indices (relative to the full size of the array) before a compaction is performed.</value>
        public double CompactingRelativeThreshold { get; set; } = 0.01;


        protected internal MultiKeyDictionary(IDictionary<T1, ISet<int>> indices1,
            IDictionary<T2, ISet<int>> indices2, IDictionary<T3, ISet<int>> indices3,
            IDictionary<T4, ISet<int>> indices4,
            IDictionary<ArrayTuple, int> fullIndex, IList<TValue> data,
            IList<T1> keys1, IList<T2> keys2, IList<T3> keys3, IList<T4> keys4,
            IList<(object, ISet<int>, int)> collapsedKeys, ISet<int> unusedIndices)
        {
            _indices1 = indices1;
            _indices2 = indices2;
            _indices3 = indices3;
            _indices4 = indices4;
            _fullIndex = fullIndex;
            _data = data;
            _keys1 = keys1;
            _keys2 = keys2;
            _keys3 = keys3;
            _keys4 = keys4;
            _collapsedKeys = collapsedKeys;
            _unusedIndices = unusedIndices;
        }
        /// <summary>
        /// Initializes an empty dictionary with four key dimensions.
        /// </summary>
        public MultiKeyDictionary() : this(new KeyValuePair<(T1, T2, T3, T4), TValue>[0]) { }

        /// <summary>
        /// Initializes the dictionary with existing entries keyed by four-value tuples.
        /// </summary>
        /// <param name="data">An enumerable of key/value pairs whose keys supply the four components of the composite key.</param>
        public MultiKeyDictionary(IEnumerable<KeyValuePair<(T1, T2, T3, T4), TValue>> data)
            : this(data.Select(x => (x.Key.Item1, x.Key.Item2, x.Key.Item3, x.Key.Item4, x.Value))) { }

        /// <summary>
        /// Initializes the dictionary with existing entries specified as tuples containing all key components and the value.
        /// </summary>
        /// <param name="data">An enumerable of tuples that provide four key components followed by the associated value.</param>
        public MultiKeyDictionary(IEnumerable<(T1, T2, T3, T4, TValue)> data)
        {
            _indices1 = new Dictionary<T1, ISet<int>>();
            _indices2 = new Dictionary<T2, ISet<int>>();
            _indices3 = new Dictionary<T3, ISet<int>>();
            _indices4 = new Dictionary<T4, ISet<int>>();
            _fullIndex = new Dictionary<ArrayTuple, int>(100);
            _data = new List<TValue>(100);
            _keys1 = new List<T1>(100);
            _keys2 = new List<T2>(100);
            _keys3 = new List<T3>(100);
            _keys4 = new List<T4>(100);
            _collapsedKeys = new List<(object, ISet<int>, int)>();
            _unusedIndices = new HashSet<int>();

            var idx = -1;
            foreach (var d in data)
            {
                ++idx;
                var (key1, key2, key3, key4, val) = d;
                _data.Add(val);
                _fullIndex[ArrayTuple.From(key1, key2, key3, key4)] = idx;
                _keys1.Add(key1);
                _keys2.Add(key2);
                _keys3.Add(key3);
                _keys4.Add(key4);
                if (!_indices1.TryGetValue(key1, out var ind1))
                {
                    ind1 = new HashSet<int>();
                    _indices1[key1] = ind1;
                }
                ind1.Add(idx);
                if (!_indices2.TryGetValue(key2, out var ind2))
                {
                    ind2 = new HashSet<int>();
                    _indices2[key2] = ind2;
                }
                ind2.Add(idx);
                if (!_indices3.TryGetValue(key3, out var ind3))
                {
                    ind3 = new HashSet<int>();
                    _indices3[key3] = ind3;
                }
                ind3.Add(idx);
                if (!_indices4.TryGetValue(key4, out var ind4))
                {
                    ind4 = new HashSet<int>();
                    _indices4[key4] = ind4;
                }
                ind4.Add(idx);
            }
        }
        /// <summary>
        /// Gets the number of stored values, accounting for unused indices and any active slice.
        /// </summary>
        /// <returns>The number of accessible entries in the dictionary or slice.</returns>
        public int Count => GetCollapsedIndexsetOrDefault()?.Count ?? (_data.Count - _unusedIndices.Count);

        /// <summary>
        /// Sets every stored value, or the values visible through the current slice, to the specified value.
        /// </summary>
        /// <param name="value">The replacement value assigned to each entry within scope.</param>
        public void Set(TValue value)
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices == null)
            {
                // set all values
                for (var i = 0; i < _data.Count; i++)
                {
                    // This method also overrides unused indices *shrug*
                    _data[i] = value;
                }
            }
            else
            {
                // set those values that are in the slice
                foreach (var index in collapsedIndices)
                    _data[index] = value;
            }
        }

        /// <summary>
        /// Removes the entry associated with the specified four-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="key4">The fourth component of the composite key.</param>
        /// <returns><c>true</c> if the entry existed and was removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        public bool Remove(T1 key1, T2 key2, T3 key3, T4 key4)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            ExceptionHandling.ThrowIfNull(key4, nameof(key4));

            var key = ArrayTuple.From(_collapsedKeys, key1, key2, key3, key4);
            if (!_fullIndex.TryGetValue(key, out var index))
            {
                return false;
            }
            _fullIndex.Remove(key);
            _unusedIndices.Add(index);
            _data[index] = default;
            _keys1[index] = default;
            _keys2[index] = default;
            _keys3[index] = default;
            _keys4[index] = default;
            for (var i = 0; i < _collapsedKeys.Count; i++)
            {
                _collapsedKeys[i].Item2.Remove(index); // we don't need to manipulate all indices > index, because we don't remove above
            }
            _indices1[key1].Remove(index);
            _indices2[key2].Remove(index);
            _indices3[key3].Remove(index);
            _indices4[key4].Remove(index);
            CheckCompact();
            return true;
        }
        /// <summary>
        /// Removes all stored combinations or, when sliced, the values visible within the active slice.
        /// </summary>
        /// <remarks>
        /// Index structures are updated to reflect removed combinations, and unused slots are compacted when necessary.
        /// </remarks>
        public void Clear()
        {
            var collapsed = GetCollapsedIndexsetOrDefault();
            if (collapsed == null)
            {
                _fullIndex.Clear();
                _data.Clear();
                _keys1.Clear();
                _keys2.Clear();
                _keys3.Clear();
                _keys4.Clear();
                _unusedIndices.Clear();
                foreach (var indices in _indices1.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _indices2.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _indices3.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _indices4.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _collapsedKeys)
                {
                    indices.Item2.Clear();
                }
            }
            else
            {
                foreach (var ind in collapsed)
                {
                    var key = ArrayTuple.From(_collapsedKeys, _keys1[ind], _keys2[ind], _keys3[ind], _keys4[ind]);
                    _fullIndex.Remove(key);
                }
                foreach (var indices in _indices1.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _indices2.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _indices3.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _indices4.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _collapsedKeys)
                {
                    indices.Item2.ExceptWith(collapsed);
                }
                // declare the indices unused and then compact
                _unusedIndices.UnionWith(collapsed);
                Compact();
            }
        }

        protected void CheckCompact()
        {
            if (_unusedIndices.Count > CompactingAbsoluteThreshold
                && _unusedIndices.Count > _data.Count * CompactingRelativeThreshold)
                Compact();
        }

        private void Compact()
        {
            if (_unusedIndices.Count == 0) return;
            var sequence = _unusedIndices.OrderByDescending(x => x).ToList();
            var offsets = new Dictionary<int, int>();
            var offset = 0;
            var offsetIndex = 0;
            for (var i = sequence.Count - 1; i >= 0; i--)
            {
                var index = sequence[i];
                if (offset > 0)
                {
                    for (var o = offsetIndex; o < index; o++)
                    {
                        offsets[o] = offset;
                    }
                }
                offsetIndex = index;
                offset++;
                _data.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys1.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys2.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys3.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys4.RemoveAt(sequence[sequence.Count - 1 - i]);
            }
            for (var o = offsetIndex; o < _data.Count + offset; o++)
            {
                offsets[o] = offset;
            }
            foreach (var kvp in _fullIndex.ToArray())
            {
                if (offsets.TryGetValue(kvp.Value, out var off))
                {
                    _fullIndex[kvp.Key] = kvp.Value - off;
                }
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(_unusedIndices);
                var reindex = indices.Item2.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.Item2.ExceptWith(reindex.Select(x => x.index));
                    indices.Item2.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices1)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices2)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices3)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices4)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            _unusedIndices.Clear();
        }

        /// <summary>
        /// Determines whether a value exists for the specified four-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="key4">The fourth component of the composite key.</param>
        /// <returns><c>true</c> if a value is associated with the provided keys; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        public bool Contains(T1 key1, T2 key2, T3 key3, T4 key4)
        {
            return _fullIndex.ContainsKey(ArrayTuple.From(_collapsedKeys, key1, key2, key3, key4));
        }

#if NET472 || NET481 || NETSTANDARD2_0
        /// <summary>
        /// Retrieves the collapsed index set when the dictionary operates on a sliced view.
        /// </summary>
        /// <returns>The set of indices representing the active slice, or <c>null</c> when unsliced.</returns>
        protected ISet<int> GetCollapsedIndexsetOrDefault()
        {
            HashSet<int> collapsedIndices = null;
#else
        /// <summary>
        /// Retrieves the collapsed index set when the dictionary operates on a sliced view.
        /// </summary>
        /// <returns>The set of indices representing the active slice, or <c>null</c> when unsliced.</returns>
        protected ISet<int>? GetCollapsedIndexsetOrDefault()
        {
            HashSet<int>? collapsedIndices = null;
#endif
            if (_collapsedKeys.Count > 0)
            {
                foreach (var collapsedSet in _collapsedKeys.Select(x => x.Item2).OrderBy(x => x.Count))
                {
                    if (collapsedIndices == null)
                        collapsedIndices = new HashSet<int>(collapsedSet);
                    else collapsedIndices.IntersectWith(collapsedSet);
                }
            }
            return collapsedIndices;
        }
        /// <summary>
        /// Enumerates values currently visible within the dictionary, respecting any active slice.
        /// </summary>
        /// <returns>An iterator that yields values for each stored key combination in scope.</returns>
        public IEnumerable<TValue> EnumerateValues()
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices == null)
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (!_unusedIndices.Contains(i))
                        yield return _data[i];
                }
            }
            else
            {
                foreach (var index in collapsedIndices)
                {
                    yield return _data[index];
                }
            }
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return EnumerateValues().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumerateValues().GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the value associated with the specified four-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="key4">The fourth component of the composite key.</param>
        /// <returns>The value stored under the specified composite key.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when retrieving a value for a key combination that has not been stored.</exception>
        public TValue this[T1 key1, T2 key2, T3 key3, T4 key4]
        {
            get
            {
                ExceptionHandling.ThrowIfNull(key1, nameof(key1));
                ExceptionHandling.ThrowIfNull(key2, nameof(key2));
                ExceptionHandling.ThrowIfNull(key3, nameof(key3));
                ExceptionHandling.ThrowIfNull(key4, nameof(key4));
                if (!_fullIndex.TryGetValue(ArrayTuple.From(_collapsedKeys, key1, key2, key3, key4), out var index))
                {
                    throw new ArgumentException($"Key combination {(key1, key2, key3, key4)} not found");
                }

                return _data[index];
            }
            set
            {
                ExceptionHandling.ThrowIfNull(key1, nameof(key1));
                ExceptionHandling.ThrowIfNull(key2, nameof(key2));
                ExceptionHandling.ThrowIfNull(key3, nameof(key3));
                ExceptionHandling.ThrowIfNull(key4, nameof(key4));

                var tupKey = ArrayTuple.From(_collapsedKeys, key1, key2, key3, key4);
                if (!_fullIndex.TryGetValue(tupKey, out var index))
                    index = -1;
                if (index < 0)
                {
                    if (!_indices1.TryGetValue(key1, out var indices1))
                    {
                        indices1 = new HashSet<int>();
                        _indices1[key1] = indices1;
                    }
                    if (!_indices2.TryGetValue(key2, out var indices2))
                    {
                        indices2 = new HashSet<int>();
                        _indices2[key2] = indices2;
                    }
                    if (!_indices3.TryGetValue(key3, out var indices3))
                    {
                        indices3 = new HashSet<int>();
                        _indices3[key3] = indices3;
                    }
                    if (!_indices4.TryGetValue(key4, out var indices4))
                    {
                        indices4 = new HashSet<int>();
                        _indices4[key4] = indices4;
                    }
                    if (_unusedIndices.Count > 0)
                    {
                        // reuse an unused index
                        index = _unusedIndices.First();
                        _unusedIndices.Remove(index);
                        _data[index] = value;
                        _keys1[index] = key1;
                        _keys2[index] = key2;
                        _keys3[index] = key3;
                        _keys4[index] = key4;
                    }
                    else
                    {
                        // make an insert at the end
                        index = _data.Count;
                        _data.Add(value);
                        _keys1.Add(key1);
                        _keys2.Add(key2);
                        _keys3.Add(key3);
                        _keys4.Add(key4);
                    }
                    _fullIndex[tupKey] = index;
                    indices1.Add(index);
                    indices2.Add(index);
                    indices3.Add(index);
                    indices4.Add(index);
                    // the data needs to be added to all collapsed dimensions
                    foreach (var f in _collapsedKeys)
                    {
                        f.Item2.Add(index);
                    }
                }
                else
                {
                    _data[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified tuple of key components.
        /// </summary>
        /// <param name="keys">A tuple describing the four components of the composite key.</param>
        /// <returns>The value stored under the provided tuple of keys.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any element of <paramref name="keys"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when retrieving a value for a key combination that has not been stored.</exception>
        public TValue this[(T1 key1, T2 key2, T3 key3, T4 key4) keys]
        {
            get => this[keys.key1, keys.key2, keys.key3, keys.key4];
            set => this[keys.key1, keys.key2, keys.key3, keys.key4] = value;
        }

        /// <summary>
        /// Adds a value associated with the specified four-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="key4">The fourth component of the composite key.</param>
        /// <param name="value">The value to associate with the provided composite key.</param>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a value already exists for the provided key combination.</exception>
        public void Add(T1 key1, T2 key2, T3 key3, T4 key4, TValue value)
        {
            if (Contains(key1, key2, key3, key4)) throw new ArgumentException($"Key already exists ({key1}, {key2}, {key3}, {key4})");
            this[key1, key2, key3, key4] = value;
        }

        /// <summary>
        /// Adds a value associated with the specified tuple of key components.
        /// </summary>
        /// <param name="keys">A tuple containing the four key components.</param>
        /// <param name="value">The value to associate with the provided composite key.</param>
        /// <exception cref="ArgumentNullException">Thrown when any element of <paramref name="keys"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a value already exists for the provided key combination.</exception>
        public void Add((T1 key1, T2 key2, T3 key3, T4 key4) keys, TValue value) => Add(keys.key1, keys.key2, keys.key3, keys.key4, value);

        /// <summary>
        /// Removes all entries that include the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove1(T1 key1)
        {
            if (!_indices1.TryGetValue(key1, out var indices1) || indices1.Count == 0)
                return 0;
            foreach (var index in indices1)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index], _keys4[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices1);
            }
            foreach (var indices in _indices2.Values)
            {
                indices.ExceptWith(indices1);
            }
            foreach (var indices in _indices3.Values)
            {
                indices.ExceptWith(indices1);
            }
            foreach (var indices in _indices4.Values)
            {
                indices.ExceptWith(indices1);
            }
            var removed = indices1.Count;
            _unusedIndices.UnionWith(indices1);
            indices1.Clear();
            CheckCompact();
            return removed;
        }

        /// <summary>
        /// Removes all entries that include the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove2(T2 key2)
        {
            if (!_indices2.TryGetValue(key2, out var indices2) || indices2.Count == 0)
                return 0;
            foreach (var index in indices2)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index], _keys4[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices2);
            }
            foreach (var indices in _indices1.Values)
            {
                indices.ExceptWith(indices2);
            }
            foreach (var indices in _indices3.Values)
            {
                indices.ExceptWith(indices2);
            }
            foreach (var indices in _indices4.Values)
            {
                indices.ExceptWith(indices2);
            }
            var removed = indices2.Count;
            _unusedIndices.UnionWith(indices2);
            indices2.Clear();
            CheckCompact();
            return removed;
        }

        /// <summary>
        /// Removes all entries that include the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove3(T3 key3)
        {
            if (!_indices3.TryGetValue(key3, out var indices3) || indices3.Count == 0)
                return 0;
            foreach (var index in indices3)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index], _keys4[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices3);
            }
            foreach (var indices in _indices1.Values)
            {
                indices.ExceptWith(indices3);
            }
            foreach (var indices in _indices2.Values)
            {
                indices.ExceptWith(indices3);
            }
            foreach (var indices in _indices4.Values)
            {
                indices.ExceptWith(indices3);
            }
            var removed = indices3.Count;
            _unusedIndices.UnionWith(indices3);
            indices3.Clear();
            CheckCompact();
            return removed;
        }

        /// <summary>
        /// Removes all entries that include the specified fourth key component.
        /// </summary>
        /// <param name="key4">The fourth key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove4(T4 key4)
        {
            if (!_indices4.TryGetValue(key4, out var indices4) || indices4.Count == 0)
                return 0;
            foreach (var index in indices4)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index], _keys4[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices4);
            }
            foreach (var indices in _indices1.Values)
            {
                indices.ExceptWith(indices4);
            }
            foreach (var indices in _indices2.Values)
            {
                indices.ExceptWith(indices4);
            }
            foreach (var indices in _indices3.Values)
            {
                indices.ExceptWith(indices4);
            }
            var removed = indices4.Count;
            _unusedIndices.UnionWith(indices4);
            indices4.Clear();
            CheckCompact();
            return removed;
        }

#if NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0
        /// <summary>
        /// Attempts to retrieve the value associated with the specified four-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="key4">The fourth component of the composite key.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key combination, if found; otherwise, the default value for <typeparamref name="TValue"/>.</param>
        /// <returns><c>true</c> when the key combination exists; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(T1 key1, T2 key2, T3 key3, T4 key4, out TValue value)
#else
        /// <inheritdoc cref="TryGetValue(T1, T2, T3, T4, out TValue)"/>
        public bool TryGetValue(T1 key1, T2 key2, T3 key3, T4 key4, out TValue? value)
#endif
        {
            if (!_fullIndex.TryGetValue(ArrayTuple.From(_collapsedKeys, key1, key2, key3, key4), out var index))
            {
                value = default;
                return false;
            }
            value = _data[index];
            return true;
        }

        /// <summary>
        /// Creates a three-dimensional slice scoped to the provided first key component.
        /// </summary>
        /// <param name="key1">The first key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T2, T3, T4, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T2, T3, T4, TValue> Slice1(T1 key1)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            var ind = _indices1[key1];
            if (ind.Count == 0) throw new ArgumentException($"key {key1} not found", nameof(key1));
            return new MultiKeyDictionary<T2, T3, T4, TValue>(
                _indices2, _indices3, _indices4,
                _fullIndex,
                _data, _keys2, _keys3, _keys4,
                _collapsedKeys.Concat(new[] { ((object)key1, ind, 0) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Creates a three-dimensional slice scoped to the provided second key component.
        /// </summary>
        /// <param name="key2">The second key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T1, T3, T4, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T1, T3, T4, TValue> Slice2(T2 key2)
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            var ind = _indices2[key2];
            if (ind.Count == 0) throw new ArgumentException($"key {key2} not found", nameof(key2));
            return new MultiKeyDictionary<T1, T3, T4, TValue>(
                _indices1, _indices3, _indices4,
                _fullIndex,
                _data, _keys1, _keys3, _keys4,
                _collapsedKeys.Concat(new[] { ((object)key2, ind, 1) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Creates a three-dimensional slice scoped to the provided third key component.
        /// </summary>
        /// <param name="key3">The third key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T1, T2, T4, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T1, T2, T4, TValue> Slice3(T3 key3)
        {
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            var ind = _indices3[key3];
            if (ind.Count == 0) throw new ArgumentException($"key {key3} not found", nameof(key3));
            return new MultiKeyDictionary<T1, T2, T4, TValue>(
                _indices1, _indices2, _indices4,
                _fullIndex,
                _data, _keys1, _keys2, _keys4,
                _collapsedKeys.Concat(new[] { ((object)key3, ind, 2) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Creates a three-dimensional slice scoped to the provided fourth key component.
        /// </summary>
        /// <param name="key4">The fourth key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T1, T2, T3, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key4"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T1, T2, T3, TValue> Slice4(T4 key4)
        {
            ExceptionHandling.ThrowIfNull(key4, nameof(key4));
            var ind = _indices4[key4];
            if (ind.Count == 0) throw new ArgumentException($"key {key4} not found", nameof(key4));
            return new MultiKeyDictionary<T1, T2, T3, TValue>(
                _indices1, _indices2, _indices3,
                _fullIndex,
                _data, _keys1, _keys2, _keys3,
                _collapsedKeys.Concat(new[] { ((object)key4, ind, 3) }).ToList(), _unusedIndices);
        }
        /// <summary>
        /// Determines whether any value exists that includes the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key1"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        public bool ContainsKey1(T1 key1)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            if (!_indices1.TryGetValue(key1, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }
        /// <summary>
        /// Determines whether any value exists that includes the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key2"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        public bool ContainsKey2(T2 key2)
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            if (!_indices2.TryGetValue(key2, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }
        /// <summary>
        /// Determines whether any value exists that includes the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key3"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        public bool ContainsKey3(T3 key3)
        {
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            if (!_indices3.TryGetValue(key3, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }
        /// <summary>
        /// Determines whether any value exists that includes the specified fourth key component.
        /// </summary>
        /// <param name="key4">The fourth key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key4"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key4"/> is <c>null</c>.</exception>
        public bool ContainsKey4(T4 key4)
        {
            ExceptionHandling.ThrowIfNull(key4, nameof(key4));
            if (!_indices4.TryGetValue(key4, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        public bool TrySlice1(T1 key1, out MultiKeyDictionary<T2, T3, T4, TValue> values)
#else
        /// <inheritdoc cref="TrySlice1(T1, out MultiKeyDictionary{T2, T3, T4, TValue})"/>
        public bool TrySlice1(T1 key1, out MultiKeyDictionary<T2, T3, T4, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            if (_indices1.TryGetValue(key1, out var indices) && indices.Count > 0)
            {
                values = Slice1(key1);
                return true;
            }
            values = default;
            return false;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        public bool TrySlice2(T2 key2, out MultiKeyDictionary<T1, T3, T4, TValue> values)
#else
        /// <inheritdoc cref="TrySlice2(T2, out MultiKeyDictionary{T1, T3, T4, TValue})"/>
        public bool TrySlice2(T2 key2, out MultiKeyDictionary<T1, T3, T4, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            if (_indices2.TryGetValue(key2, out var indices) && indices.Count > 0)
            {
                values = Slice2(key2);
                return true;
            }
            values = default;
            return false;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        public bool TrySlice3(T3 key3, out MultiKeyDictionary<T1, T2, T4, TValue> values)
#else
        /// <inheritdoc cref="TrySlice3(T3, out MultiKeyDictionary{T1, T2, T4, TValue})"/>
        public bool TrySlice3(T3 key3, out MultiKeyDictionary<T1, T2, T4, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            if (_indices3.TryGetValue(key3, out var indices) && indices.Count > 0)
            {
                values = Slice3(key3);
                return true;
            }
            values = default;
            return false;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified fourth key component.
        /// </summary>
        /// <param name="key4">The fourth key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key4"/> is <c>null</c>.</exception>
        public bool TrySlice4(T4 key4, out MultiKeyDictionary<T1, T2, T3, TValue> values)
#else
        /// <inheritdoc cref="TrySlice4(T4, out MultiKeyDictionary{T1, T2, T3, TValue})"/>
        public bool TrySlice4(T4 key4, out MultiKeyDictionary<T1, T2, T3, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key4, nameof(key4));
            if (_indices4.TryGetValue(key4, out var indices) && indices.Count > 0)
            {
                values = Slice4(key4);
                return true;
            }
            values = default;
            return false;
        }

        /// <summary>
        /// Enumerates the stored entries as tuples containing all four key components and the associated value.
        /// </summary>
        /// <returns>An iterator that yields tuples of each composite key and its corresponding value.</returns>
        public IEnumerable<(T1, T2, T3, T4, TValue)> Enumerate()
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices != null)
            {
                foreach (var i in collapsedIndices)
                {
                    yield return ((T1)_keys1[i], (T2)_keys2[i], (T3)_keys3[i], (T4)_keys4[i], _data[i]);
                }
            }
            else
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (!_unusedIndices.Contains(i))
                        yield return ((T1)_keys1[i], (T2)_keys2[i], (T3)_keys3[i], (T4)_keys4[i], _data[i]);
                }
            }
        }

        /// <summary>
        /// Materializes the dictionary into a standard <see cref="Dictionary{TKey, TValue}"/> using tuple-based composite keys.
        /// </summary>
        /// <returns>A dictionary containing every stored key combination and its associated value.</returns>
        public Dictionary<(T1, T2, T3, T4), TValue> ToDictionary() => Enumerate().ToDictionary(x => (x.Item1, x.Item2, x.Item3, x.Item4), x => x.Item5);
    }

    /// <summary>
    /// Represents a dictionary that indexes values by three distinct key components with optional slicing support.
    /// </summary>
    /// <typeparam name="T1">The type of the first key component.</typeparam>
    /// <typeparam name="T2">The type of the second key component.</typeparam>
    /// <typeparam name="T3">The type of the third key component.</typeparam>
    /// <typeparam name="TValue">The type of values stored in the dictionary.</typeparam>
    public class MultiKeyDictionary<T1, T2, T3, TValue> : IEnumerable<TValue>
#if !(NET472 || NET481 || NETSTANDARD2_0)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
#endif
    {
        private const int KEYS = 4;
        private IDictionary<T1, ISet<int>> _indices1;
        private IDictionary<T2, ISet<int>> _indices2;
        private IDictionary<T3, ISet<int>> _indices3;
        private IDictionary<ArrayTuple, int> _fullIndex;
        private IList<TValue> _data;
        private IList<T1> _keys1;
        private IList<T2> _keys2;
        private IList<T3> _keys3;
        private IList<(object, ISet<int>, int)> _collapsedKeys;
        private ISet<int> _unusedIndices;

        /// <summary>
        /// Removals do not actually remove data from the table, but just delete the index.
        /// If a lot of Remove operations are performed, the table will become very sparsely populated with actual data.
        /// This setting controls the absolute number of unused indices that will trigger a compaction.
        /// Both this and the relative threshold must be met for a compaction to occur.
        /// </summary>
        /// <value>The number of unused indices before a compaction is performed.</value>
        public int CompactingAbsoluteThreshold { get; set; } = 1000;
        /// <summary>
        /// Removals do not actually remove data from the table, but just delete the index.
        /// If a lot of Remove operations are performed, the table will become very sparsely populated with actual data.
        /// This setting controls the relative number of unused indices that will trigger a compaction.
        /// Both this and the absolute threshold must be met for a compaction to occur.
        /// </summary>
        /// <value>The relative number of unused indices (relative to the full size of the array) before a compaction is performed.</value>
        public double CompactingRelativeThreshold { get; set; } = 0.01;


        protected internal MultiKeyDictionary(IDictionary<T1, ISet<int>> indices1,
            IDictionary<T2, ISet<int>> indices2, IDictionary<T3, ISet<int>> indices3,
            IDictionary<ArrayTuple, int> fullIndex, IList<TValue> data,
            IList<T1> keys1, IList<T2> keys2, IList<T3> keys3,
            IList<(object, ISet<int>, int)> collapsedKeys, ISet<int> unusedIndices)
        {
            _indices1 = indices1;
            _indices2 = indices2;
            _indices3 = indices3;
            _fullIndex = fullIndex;
            _data = data;
            _keys1 = keys1;
            _keys2 = keys2;
            _keys3 = keys3;
            _collapsedKeys = collapsedKeys;
            _unusedIndices = unusedIndices;
        }
        /// <summary>
        /// Initializes an empty dictionary with three key dimensions.
        /// </summary>
        public MultiKeyDictionary() : this(new KeyValuePair<(T1, T2, T3), TValue>[0]) { }

        /// <summary>
        /// Initializes the dictionary with existing entries keyed by three-value tuples.
        /// </summary>
        /// <param name="data">An enumerable of key/value pairs whose keys supply the three components of the composite key.</param>
        public MultiKeyDictionary(IEnumerable<KeyValuePair<(T1, T2, T3), TValue>> data)
            : this(data.Select(x => (x.Key.Item1, x.Key.Item2, x.Key.Item3, x.Value))) { }

        /// <summary>
        /// Initializes the dictionary with existing entries specified as tuples containing all key components and the value.
        /// </summary>
        /// <param name="data">An enumerable of tuples that provide three key components followed by the associated value.</param>
        public MultiKeyDictionary(IEnumerable<(T1, T2, T3, TValue)> data)
        {
            _indices1 = new Dictionary<T1, ISet<int>>();
            _indices2 = new Dictionary<T2, ISet<int>>();
            _indices3 = new Dictionary<T3, ISet<int>>();
            _fullIndex = new Dictionary<ArrayTuple, int>(100);
            _data = new List<TValue>(100);
            _keys1 = new List<T1>(100);
            _keys2 = new List<T2>(100);
            _keys3 = new List<T3>(100);
            _collapsedKeys = new List<(object, ISet<int>, int)>();
            _unusedIndices = new HashSet<int>();

            var idx = -1;
            foreach (var d in data)
            {
                ++idx;
                var (key1, key2, key3, val) = d;
                _data.Add(val);
                _fullIndex[ArrayTuple.From(key1, key2, key3)] = idx;
                _keys1.Add(key1);
                _keys2.Add(key2);
                _keys3.Add(key3);
                if (!_indices1.TryGetValue(key1, out var ind1))
                {
                    ind1 = new HashSet<int>();
                    _indices1[key1] = ind1;
                }
                ind1.Add(idx);
                if (!_indices2.TryGetValue(key2, out var ind2))
                {
                    ind2 = new HashSet<int>();
                    _indices2[key2] = ind2;
                }
                ind2.Add(idx);
                if (!_indices3.TryGetValue(key3, out var ind3))
                {
                    ind3 = new HashSet<int>();
                    _indices3[key3] = ind3;
                }
                ind3.Add(idx);
            }
        }
        /// <summary>
        /// Gets the number of stored values, accounting for unused indices and any active slice.
        /// </summary>
        /// <returns>The number of accessible entries in the dictionary or slice.</returns>
        public int Count => GetCollapsedIndexsetOrDefault()?.Count ?? (_data.Count - _unusedIndices.Count);

        /// <summary>
        /// Sets every stored value, or the values visible through the current slice, to the specified value.
        /// </summary>
        /// <param name="value">The replacement value assigned to each entry within scope.</param>
        public void Set(TValue value)
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices == null)
            {
                // set all values
                for (var i = 0; i < _data.Count; i++)
                {
                    // This method also overrides unused indices *shrug*
                    _data[i] = value;
                }
            }
            else
            {
                // set those values that are in the slice
                foreach (var index in collapsedIndices)
                    _data[index] = value;
            }
        }

        /// <summary>
        /// Removes the entry associated with the specified three-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <returns><c>true</c> if the entry existed and was removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        public bool Remove(T1 key1, T2 key2, T3 key3)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));

            var key = ArrayTuple.From(_collapsedKeys, key1, key2, key3);
            if (!_fullIndex.TryGetValue(key, out var index))
            {
                return false;
            }
            _fullIndex.Remove(key);
            _unusedIndices.Add(index);
            _data[index] = default;
            _keys1[index] = default;
            _keys2[index] = default;
            _keys3[index] = default;
            for (var i = 0; i < _collapsedKeys.Count; i++)
            {
                _collapsedKeys[i].Item2.Remove(index); // we don't need to manipulate all indices > index, because we don't remove above
            }
            _indices1[key1].Remove(index);
            _indices2[key2].Remove(index);
            _indices3[key3].Remove(index);
            CheckCompact();
            return true;
        }

        /// <summary>
        /// Removes all stored combinations or, when sliced, the values visible within the active slice.
        /// </summary>
        /// <remarks>
        /// Index structures are updated to reflect removed combinations, and unused slots are compacted when necessary.
        /// </remarks>
        public void Clear()
        {
            var collapsed = GetCollapsedIndexsetOrDefault();
            if (collapsed == null)
            {
                _fullIndex.Clear();
                _data.Clear();
                _keys1.Clear();
                _keys2.Clear();
                _keys3.Clear();
                _unusedIndices.Clear();
                foreach (var indices in _indices1.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _indices2.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _indices3.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _collapsedKeys)
                {
                    indices.Item2.Clear();
                }
            }
            else
            {
                foreach (var ind in collapsed)
                {
                    var key = ArrayTuple.From(_collapsedKeys, _keys1[ind], _keys2[ind], _keys3[ind]);
                    _fullIndex.Remove(key);
                }
                foreach (var indices in _indices1.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _indices2.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _indices3.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _collapsedKeys)
                {
                    indices.Item2.ExceptWith(collapsed);
                }
                // declare the indices unused and then compact
                _unusedIndices.UnionWith(collapsed);
                Compact();
            }
        }

        protected void CheckCompact()
        {
            if (_unusedIndices.Count > CompactingAbsoluteThreshold
                && _unusedIndices.Count > _data.Count * CompactingRelativeThreshold)
                Compact();
        }

        private void Compact()
        {
            if (_unusedIndices.Count == 0) return;
            var sequence = _unusedIndices.OrderByDescending(x => x).ToList();
            var offsets = new Dictionary<int, int>();
            var offset = 0;
            var offsetIndex = 0;
            for (var i = sequence.Count - 1; i >= 0; i--)
            {
                var index = sequence[i];
                if (offset > 0)
                {
                    for (var o = offsetIndex; o < index; o++)
                    {
                        offsets[o] = offset;
                    }
                }
                offsetIndex = index;
                offset++;
                _data.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys1.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys2.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys3.RemoveAt(sequence[sequence.Count - 1 - i]);
            }
            for (var o = offsetIndex; o < _data.Count + offset; o++)
            {
                offsets[o] = offset;
            }
            foreach (var kvp in _fullIndex.ToArray())
            {
                if (offsets.TryGetValue(kvp.Value, out var off))
                {
                    _fullIndex[kvp.Key] = kvp.Value - off;
                }
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(_unusedIndices);
                var reindex = indices.Item2.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.Item2.ExceptWith(reindex.Select(x => x.index));
                    indices.Item2.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices1)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices2)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices3)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            _unusedIndices.Clear();
        }

        /// <summary>
        /// Determines whether a value exists for the specified three-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <returns><c>true</c> if a value is associated with the provided keys; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        public bool Contains(T1 key1, T2 key2, T3 key3)
        {
            return _fullIndex.ContainsKey(ArrayTuple.From(_collapsedKeys, key1, key2, key3));
        }

#if NET472 || NET481 || NETSTANDARD2_0
        /// <summary>
        /// Retrieves the collapsed index set when the dictionary operates on a sliced view.
        /// </summary>
        /// <returns>The set of indices representing the active slice, or <c>null</c> when unsliced.</returns>
        protected ISet<int> GetCollapsedIndexsetOrDefault()
        {
            HashSet<int> collapsedIndices = null;
#else
        /// <summary>
        /// Retrieves the collapsed index set when the dictionary operates on a sliced view.
        /// </summary>
        /// <returns>The set of indices representing the active slice, or <c>null</c> when unsliced.</returns>
        protected ISet<int>? GetCollapsedIndexsetOrDefault()
        {
            HashSet<int>? collapsedIndices = null;
#endif
            if (_collapsedKeys.Count > 0)
            {
                foreach (var collapsedSet in _collapsedKeys.Select(x => x.Item2).OrderBy(x => x.Count))
                {
                    if (collapsedIndices == null)
                        collapsedIndices = new HashSet<int>(collapsedSet);
                    else collapsedIndices.IntersectWith(collapsedSet);
                }
            }
            return collapsedIndices;
        }
        /// <summary>
        /// Enumerates values currently visible within the dictionary, respecting any active slice.
        /// </summary>
        /// <returns>An iterator that yields values for each stored key combination in scope.</returns>
        public IEnumerable<TValue> EnumerateValues()
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices == null)
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (!_unusedIndices.Contains(i))
                        yield return _data[i];
                }
            }
            else
            {
                foreach (var index in collapsedIndices)
                {
                    yield return _data[index];
                }
            }
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return EnumerateValues().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumerateValues().GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the value associated with the specified three-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <returns>The value stored under the specified composite key.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when retrieving a value for a key combination that has not been stored.</exception>
        public TValue this[T1 key1, T2 key2, T3 key3]
        {
            get
            {
                ExceptionHandling.ThrowIfNull(key1, nameof(key1));
                ExceptionHandling.ThrowIfNull(key2, nameof(key2));
                ExceptionHandling.ThrowIfNull(key3, nameof(key3));
                if (!_fullIndex.TryGetValue(ArrayTuple.From(_collapsedKeys, key1, key2, key3), out var index))
                {
                    throw new ArgumentException($"Key combination {(key1, key2, key3)} not found");
                }

                return _data[index];
            }
            set
            {
                ExceptionHandling.ThrowIfNull(key1, nameof(key1));
                ExceptionHandling.ThrowIfNull(key2, nameof(key2));
                ExceptionHandling.ThrowIfNull(key3, nameof(key3));

                var tupKey = ArrayTuple.From(_collapsedKeys, key1, key2, key3);
                if (!_fullIndex.TryGetValue(tupKey, out var index))
                    index = -1;
                if (index < 0)
                {
                    if (!_indices1.TryGetValue(key1, out var indices1))
                    {
                        indices1 = new HashSet<int>();
                        _indices1[key1] = indices1;
                    }
                    if (!_indices2.TryGetValue(key2, out var indices2))
                    {
                        indices2 = new HashSet<int>();
                        _indices2[key2] = indices2;
                    }
                    if (!_indices3.TryGetValue(key3, out var indices3))
                    {
                        indices3 = new HashSet<int>();
                        _indices3[key3] = indices3;
                    }
                    if (_unusedIndices.Count > 0)
                    {
                        // reuse an unused index
                        index = _unusedIndices.First();
                        _unusedIndices.Remove(index);
                        _data[index] = value;
                        _keys1[index] = key1;
                        _keys2[index] = key2;
                        _keys3[index] = key3;
                    }
                    else
                    {
                        // make an insert at the end
                        index = _data.Count;
                        _data.Add(value);
                        _keys1.Add(key1);
                        _keys2.Add(key2);
                        _keys3.Add(key3);
                    }
                    _fullIndex[tupKey] = index;
                    indices1.Add(index);
                    indices2.Add(index);
                    indices3.Add(index);
                    // the data needs to be added to all collapsed dimensions
                    foreach (var f in _collapsedKeys)
                    {
                        f.Item2.Add(index);
                    }
                }
                else
                {
                    _data[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified tuple of key components.
        /// </summary>
        /// <param name="keys">A tuple describing the three components of the composite key.</param>
        /// <returns>The value stored under the provided tuple of keys.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any element of <paramref name="keys"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when retrieving a value for a key combination that has not been stored.</exception>
        public TValue this[(T1 key1, T2 key2, T3 key3) keys]
        {
            get => this[keys.key1, keys.key2, keys.key3];
            set => this[keys.key1, keys.key2, keys.key3] = value;
        }

        /// <summary>
        /// Adds a value associated with the specified three-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="value">The value to associate with the provided composite key.</param>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a value already exists for the provided key combination.</exception>
        public void Add(T1 key1, T2 key2, T3 key3, TValue value)
        {
            if (Contains(key1, key2, key3)) throw new ArgumentException($"Key already exists ({key1}, {key2}, {key3})");
            this[key1, key2, key3] = value;
        }

        /// <summary>
        /// Adds a value associated with the specified tuple of key components.
        /// </summary>
        /// <param name="keys">A tuple containing the three key components.</param>
        /// <param name="value">The value to associate with the provided composite key.</param>
        /// <exception cref="ArgumentNullException">Thrown when any element of <paramref name="keys"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a value already exists for the provided key combination.</exception>
        public void Add((T1 key1, T2 key2, T3 key3) keys, TValue value) => Add(keys.key1, keys.key2, keys.key3, value);

        /// <summary>
        /// Removes all entries that include the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove1(T1 key1)
        {
            if (!_indices1.TryGetValue(key1, out var indices1) || indices1.Count == 0)
                return 0;
            foreach (var index in indices1)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices1);
            }
            foreach (var indices in _indices2.Values)
            {
                indices.ExceptWith(indices1);
            }
            foreach (var indices in _indices3.Values)
            {
                indices.ExceptWith(indices1);
            }
            var removed = indices1.Count;
            _unusedIndices.UnionWith(indices1);
            indices1.Clear();
            CheckCompact();
            return removed;
        }

        /// <summary>
        /// Removes all entries that include the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove2(T2 key2)
        {
            if (!_indices2.TryGetValue(key2, out var indices2) || indices2.Count == 0)
                return 0;
            foreach (var index in indices2)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices2);
            }
            foreach (var indices in _indices1.Values)
            {
                indices.ExceptWith(indices2);
            }
            foreach (var indices in _indices3.Values)
            {
                indices.ExceptWith(indices2);
            }
            var removed = indices2.Count;
            _unusedIndices.UnionWith(indices2);
            indices2.Clear();
            CheckCompact();
            return removed;
        }

        /// <summary>
        /// Removes all entries that include the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove3(T3 key3)
        {
            if (!_indices3.TryGetValue(key3, out var indices3) || indices3.Count == 0)
                return 0;
            foreach (var index in indices3)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index], _keys3[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices3);
            }
            foreach (var indices in _indices1.Values)
            {
                indices.ExceptWith(indices3);
            }
            foreach (var indices in _indices2.Values)
            {
                indices.ExceptWith(indices3);
            }
            var removed = indices3.Count;
            _unusedIndices.UnionWith(indices3);
            indices3.Clear();
            CheckCompact();
            return removed;
        }

#if NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0
        /// <summary>
        /// Attempts to retrieve the value associated with the specified three-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="key3">The third component of the composite key.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key combination, if found; otherwise, the default value for <typeparamref name="TValue"/>.</param>
        /// <returns><c>true</c> when the key combination exists; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(T1 key1, T2 key2, T3 key3, out TValue value)
#else
        /// <inheritdoc cref="TryGetValue(T1, T2, T3, out TValue)"/>
        public bool TryGetValue(T1 key1, T2 key2, T3 key3, out TValue? value)
#endif
        {
            if (!_fullIndex.TryGetValue(ArrayTuple.From(_collapsedKeys, key1, key2, key3), out var index))
            {
                value = default;
                return false;
            }
            value = _data[index];
            return true;
        }

        /// <summary>
        /// Creates a two-dimensional slice scoped to the provided first key component.
        /// </summary>
        /// <param name="key1">The first key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T2, T3, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T2, T3, TValue> Slice1(T1 key1)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            var ind = _indices1[key1];
            if (ind.Count == 0) throw new ArgumentException($"key {key1} not found", nameof(key1));
            return new MultiKeyDictionary<T2, T3, TValue>(
                _indices2, _indices3,
                _fullIndex,
                _data, _keys2, _keys3,
                _collapsedKeys.Concat(new[] { ((object)key1, ind, 0) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Creates a two-dimensional slice scoped to the provided second key component.
        /// </summary>
        /// <param name="key2">The second key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T1, T3, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T1, T3, TValue> Slice2(T2 key2)
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            var ind = _indices2[key2];
            if (ind.Count == 0) throw new ArgumentException($"key {key2} not found", nameof(key2));
            return new MultiKeyDictionary<T1, T3, TValue>(
                _indices1, _indices3,
                _fullIndex,
                _data, _keys1, _keys3,
                _collapsedKeys.Concat(new[] { ((object)key2, ind, 1) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Creates a two-dimensional slice scoped to the provided third key component.
        /// </summary>
        /// <param name="key3">The third key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T1, T2, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T1, T2, TValue> Slice3(T3 key3)
        {
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            var ind = _indices3[key3];
            if (ind.Count == 0) throw new ArgumentException($"key {key3} not found", nameof(key3));
            return new MultiKeyDictionary<T1, T2, TValue>(
                _indices1, _indices2,
                _fullIndex,
                _data, _keys1, _keys2,
                _collapsedKeys.Concat(new[] { ((object)key3, ind, 2) }).ToList(), _unusedIndices);
        }
        /// <summary>
        /// Determines whether any value exists that includes the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key1"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        public bool ContainsKey1(T1 key1)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            if (!_indices1.TryGetValue(key1, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }
        /// <summary>
        /// Determines whether any value exists that includes the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key2"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        public bool ContainsKey2(T2 key2)
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            if (!_indices2.TryGetValue(key2, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }
        /// <summary>
        /// Determines whether any value exists that includes the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key3"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        public bool ContainsKey3(T3 key3)
        {
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            if (!_indices3.TryGetValue(key3, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        public bool TrySlice1(T1 key1, out MultiKeyDictionary<T2, T3, TValue> values)
#else
        /// <inheritdoc cref="TrySlice1(T1, out MultiKeyDictionary{T2, T3, TValue})"/>
        public bool TrySlice1(T1 key1, out MultiKeyDictionary<T2, T3, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            if (_indices1.TryGetValue(key1, out var indices) && indices.Count > 0)
            {
                values = Slice1(key1);
                return true;
            }
            values = default;
            return false;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        public bool TrySlice2(T2 key2, out MultiKeyDictionary<T1, T3, TValue> values)
#else
        /// <inheritdoc cref="TrySlice2(T2, out MultiKeyDictionary{T1, T3, TValue})"/>
        public bool TrySlice2(T2 key2, out MultiKeyDictionary<T1, T3, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            if (_indices2.TryGetValue(key2, out var indices) && indices.Count > 0)
            {
                values = Slice2(key2);
                return true;
            }
            values = default;
            return false;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified third key component.
        /// </summary>
        /// <param name="key3">The third key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key3"/> is <c>null</c>.</exception>
        public bool TrySlice3(T3 key3, out MultiKeyDictionary<T1, T2, TValue> values)
#else
        /// <inheritdoc cref="TrySlice3(T3, out MultiKeyDictionary{T1, T2, TValue})"/>
        public bool TrySlice3(T3 key3, out MultiKeyDictionary<T1, T2, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key3, nameof(key3));
            if (_indices3.TryGetValue(key3, out var indices) && indices.Count > 0)
            {
                values = Slice3(key3);
                return true;
            }
            values = default;
            return false;
        }

        /// <summary>
        /// Enumerates the stored entries as tuples containing all three key components and the associated value.
        /// </summary>
        /// <returns>An iterator that yields tuples of each composite key and its corresponding value.</returns>
        public IEnumerable<(T1, T2, T3, TValue)> Enumerate()
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices != null)
            {
                foreach (var i in collapsedIndices)
                {
                    yield return ((T1)_keys1[i], (T2)_keys2[i], (T3)_keys3[i], _data[i]);
                }
            }
            else
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (!_unusedIndices.Contains(i))
                        yield return ((T1)_keys1[i], (T2)_keys2[i], (T3)_keys3[i], _data[i]);
                }
            }
        }

        /// <summary>
        /// Materializes the dictionary into a standard <see cref="Dictionary{TKey, TValue}"/> using tuple-based composite keys.
        /// </summary>
        /// <returns>A dictionary containing every stored key combination and its associated value.</returns>
        public Dictionary<(T1, T2, T3), TValue> ToDictionary() => Enumerate().ToDictionary(x => (x.Item1, x.Item2, x.Item3), x => x.Item4);
    }

    /// <summary>
    /// Represents a dictionary that indexes values by two distinct key components with optional slicing support.
    /// </summary>
    /// <typeparam name="T1">The type of the first key component.</typeparam>
    /// <typeparam name="T2">The type of the second key component.</typeparam>
    /// <typeparam name="TValue">The type of values stored in the dictionary.</typeparam>
    public class MultiKeyDictionary<T1, T2, TValue> : IEnumerable<TValue>
#if !(NET472 || NET481 || NETSTANDARD2_0)
        where T1 : notnull
        where T2 : notnull
#endif
    {
        private IDictionary<T1, ISet<int>> _indices1;
        private IDictionary<T2, ISet<int>> _indices2;
        private IDictionary<ArrayTuple, int> _fullIndex;
        private IList<TValue> _data;
        private IList<T1> _keys1;
        private IList<T2> _keys2;
        private IList<(object, ISet<int>, int)> _collapsedKeys;
        private ISet<int> _unusedIndices;

        /// <summary>
        /// Removals do not actually remove data from the table, but just delete the index.
        /// If a lot of Remove operations are performed, the table will become very sparsely populated with actual data.
        /// This setting controls the absolute number of unused indices that will trigger a compaction.
        /// Both this and the relative threshold must be met for a compaction to occur.
        /// </summary>
        /// <value>The number of unused indices before a compaction is performed.</value>
        public int CompactingAbsoluteThreshold { get; set; } = 1000;
        /// <summary>
        /// Removals do not actually remove data from the table, but just delete the index.
        /// If a lot of Remove operations are performed, the table will become very sparsely populated with actual data.
        /// This setting controls the relative number of unused indices that will trigger a compaction.
        /// Both this and the absolute threshold must be met for a compaction to occur.
        /// </summary>
        /// <value>The relative number of unused indices (relative to the full size of the array) before a compaction is performed.</value>
        public double CompactingRelativeThreshold { get; set; } = 0.01;


        protected internal MultiKeyDictionary(IDictionary<T1, ISet<int>> indices1,
            IDictionary<T2, ISet<int>> indices2,
            IDictionary<ArrayTuple, int> fullIndex, IList<TValue> data,
            IList<T1> keys1, IList<T2> keys2,
            IList<(object, ISet<int>, int)> collapsedKeys, ISet<int> unusedIndices)
        {
            _indices1 = indices1;
            _indices2 = indices2;
            _fullIndex = fullIndex;
            _data = data;
            _keys1 = keys1;
            _keys2 = keys2;
            _collapsedKeys = collapsedKeys;
            _unusedIndices = unusedIndices;
        }
        /// <summary>
        /// Initializes an empty dictionary with two key dimensions.
        /// </summary>
        public MultiKeyDictionary() : this(new KeyValuePair<(T1, T2), TValue>[0]) { }

        /// <summary>
        /// Initializes the dictionary with existing entries keyed by two-value tuples.
        /// </summary>
        /// <param name="data">An enumerable of key/value pairs whose keys supply the two components of the composite key.</param>
        public MultiKeyDictionary(IEnumerable<KeyValuePair<(T1, T2), TValue>> data)
            : this(data.Select(x => (x.Key.Item1, x.Key.Item2, x.Value))) { }

        /// <summary>
        /// Initializes the dictionary with existing entries specified as tuples containing both key components and the value.
        /// </summary>
        /// <param name="data">An enumerable of tuples that provide two key components followed by the associated value.</param>
        public MultiKeyDictionary(IEnumerable<(T1, T2, TValue)> data)
        {
            _indices1 = new Dictionary<T1, ISet<int>>();
            _indices2 = new Dictionary<T2, ISet<int>>();
            _fullIndex = new Dictionary<ArrayTuple, int>(100);
            _data = new List<TValue>(100);
            _keys1 = new List<T1>(100);
            _keys2 = new List<T2>(100);
            _collapsedKeys = new List<(object, ISet<int>, int)>();
            _unusedIndices = new HashSet<int>();

            var idx = -1;
            foreach (var d in data)
            {
                ++idx;
                var (key1, key2, val) = d;
                _data.Add(val);
                _fullIndex[ArrayTuple.From(key1, key2)] = idx;
                _keys1.Add(key1);
                _keys2.Add(key2);
                if (!_indices1.TryGetValue(key1, out var ind1))
                {
                    ind1 = new HashSet<int>();
                    _indices1[key1] = ind1;
                }
                ind1.Add(idx);
                if (!_indices2.TryGetValue(key2, out var ind2))
                {
                    ind2 = new HashSet<int>();
                    _indices2[key2] = ind2;
                }
                ind2.Add(idx);
            }
        }
        /// <summary>
        /// Gets the number of stored values, accounting for unused indices and any active slice.
        /// </summary>
        /// <returns>The number of accessible entries in the dictionary or slice.</returns>
        public int Count => GetCollapsedIndexsetOrDefault()?.Count ?? (_data.Count - _unusedIndices.Count);

        /// <summary>
        /// Sets every stored value, or the values visible through the current slice, to the specified value.
        /// </summary>
        /// <param name="value">The replacement value assigned to each entry within scope.</param>
        public void Set(TValue value)
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices == null)
            {
                // set all values
                for (var i = 0; i < _data.Count; i++)
                {
                    // This method also overrides unused indices *shrug*
                    _data[i] = value;
                }
            }
            else
            {
                // set those values that are in the slice
                foreach (var index in collapsedIndices)
                    _data[index] = value;
            }
        }

        /// <summary>
        /// Removes the entry associated with the specified two-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <returns><c>true</c> if the entry existed and was removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        public bool Remove(T1 key1, T2 key2)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));

            var key = ArrayTuple.From(_collapsedKeys, key1, key2);
            if (!_fullIndex.TryGetValue(key, out var index))
            {
                return false;
            }
            _fullIndex.Remove(key);
            _unusedIndices.Add(index);
            _data[index] = default;
            _keys1[index] = default;
            _keys2[index] = default;
            for (var i = 0; i < _collapsedKeys.Count; i++)
            {
                _collapsedKeys[i].Item2.Remove(index); // we don't need to manipulate all indices > index, because we don't remove above
            }
            _indices1[key1].Remove(index);
            _indices2[key2].Remove(index);
            CheckCompact();
            return true;
        }

        /// <summary>
        /// Removes all stored combinations or, when sliced, the values visible within the active slice.
        /// </summary>
        /// <remarks>
        /// Index structures are updated to reflect removed combinations, and unused slots are compacted when necessary.
        /// </remarks>
        public void Clear()
        {
            var collapsed = GetCollapsedIndexsetOrDefault();
            if (collapsed == null)
            {
                _fullIndex.Clear();
                _data.Clear();
                _keys1.Clear();
                _keys2.Clear();
                _unusedIndices.Clear();
                foreach (var indices in _indices1.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _indices2.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _collapsedKeys)
                {
                    indices.Item2.Clear();
                }
            }
            else
            {
                foreach (var ind in collapsed)
                {
                    var key = ArrayTuple.From(_collapsedKeys, _keys1[ind], _keys2[ind]);
                    _fullIndex.Remove(key);
                }
                foreach (var indices in _indices1.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _indices2.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _collapsedKeys)
                {
                    indices.Item2.ExceptWith(collapsed);
                }
                // declare the indices unused and then compact
                _unusedIndices.UnionWith(collapsed);
                Compact();
            }
        }

        protected void CheckCompact()
        {
            if (_unusedIndices.Count > CompactingAbsoluteThreshold
                && _unusedIndices.Count > _data.Count * CompactingRelativeThreshold)
                Compact();
        }

        private void Compact()
        {
            if (_unusedIndices.Count == 0) return;
            var sequence = _unusedIndices.OrderByDescending(x => x).ToList();
            var offsets = new Dictionary<int, int>();
            var offset = 0;
            var offsetIndex = 0;
            for (var i = sequence.Count - 1; i >= 0; i--)
            {
                var index = sequence[i];
                if (offset > 0)
                {
                    for (var o = offsetIndex; o < index; o++)
                    {
                        offsets[o] = offset;
                    }
                }
                offsetIndex = index;
                offset++;
                _data.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys1.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys2.RemoveAt(sequence[sequence.Count - 1 - i]);
            }
            for (var o = offsetIndex; o < _data.Count + offset; o++)
            {
                offsets[o] = offset;
            }
            foreach (var kvp in _fullIndex.ToArray())
            {
                if (offsets.TryGetValue(kvp.Value, out var off))
                {
                    _fullIndex[kvp.Key] = kvp.Value - off;
                }
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(_unusedIndices);
                var reindex = indices.Item2.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.Item2.ExceptWith(reindex.Select(x => x.index));
                    indices.Item2.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices1)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices2)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            _unusedIndices.Clear();
        }

        /// <summary>
        /// Determines whether a value exists for the specified two-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <returns><c>true</c> if a value is associated with the provided keys; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        public bool Contains(T1 key1, T2 key2)
        {
            return _fullIndex.ContainsKey(ArrayTuple.From(_collapsedKeys, key1, key2));
        }

#if NET472 || NET481 || NETSTANDARD2_0
        protected ISet<int> GetCollapsedIndexsetOrDefault()
        {
            HashSet<int> collapsedIndices = null;
#else
        protected ISet<int>? GetCollapsedIndexsetOrDefault()
        {
            HashSet<int>? collapsedIndices = null;
#endif
            if (_collapsedKeys.Count > 0)
            {
                foreach (var collapsedSet in _collapsedKeys.Select(x => x.Item2).OrderBy(x => x.Count))
                {
                    if (collapsedIndices == null)
                        collapsedIndices = new HashSet<int>(collapsedSet);
                    else collapsedIndices.IntersectWith(collapsedSet);
                }
            }
            return collapsedIndices;
        }

        /// <summary>
        /// Enumerates values currently visible within the dictionary, respecting any active slice.
        /// </summary>
        /// <returns>An iterator that yields values for each stored key combination in scope.</returns>
        public IEnumerable<TValue> EnumerateValues()
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices == null)
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (!_unusedIndices.Contains(i))
                        yield return _data[i];
                }
            }
            else
            {
                foreach (var index in collapsedIndices)
                {
                    yield return _data[index];
                }
            }
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return EnumerateValues().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumerateValues().GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the value associated with the specified two-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <returns>The value stored under the specified composite key.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when retrieving a value for a key combination that has not been stored.</exception>
        public TValue this[T1 key1, T2 key2]
        {
            get
            {
                ExceptionHandling.ThrowIfNull(key1, nameof(key1));
                ExceptionHandling.ThrowIfNull(key2, nameof(key2));
                if (!_fullIndex.TryGetValue(ArrayTuple.From(_collapsedKeys, key1, key2), out var index))
                {
                    throw new ArgumentException($"Key combination {(key1, key2)} not found");
                }

                return _data[index];
            }
            set
            {
                ExceptionHandling.ThrowIfNull(key1, nameof(key1));
                ExceptionHandling.ThrowIfNull(key2, nameof(key2));

                var tupKey = ArrayTuple.From(_collapsedKeys, key1, key2);
                if (!_fullIndex.TryGetValue(tupKey, out var index))
                    index = -1;
                if (index < 0)
                {
                    if (!_indices1.TryGetValue(key1, out var indices1))
                    {
                        indices1 = new HashSet<int>();
                        _indices1[key1] = indices1;
                    }
                    if (!_indices2.TryGetValue(key2, out var indices2))
                    {
                        indices2 = new HashSet<int>();
                        _indices2[key2] = indices2;
                    }
                    if (_unusedIndices.Count > 0)
                    {
                        // reuse an unused index
                        index = _unusedIndices.First();
                        _unusedIndices.Remove(index);
                        _data[index] = value;
                        _keys1[index] = key1;
                        _keys2[index] = key2;
                    }
                    else
                    {
                        // make an insert at the end
                        index = _data.Count;
                        _data.Add(value);
                        _keys1.Add(key1);
                        _keys2.Add(key2);
                    }
                    _fullIndex[tupKey] = index;
                    indices1.Add(index);
                    indices2.Add(index);
                    // the data needs to be added to all collapsed dimensions
                    foreach (var f in _collapsedKeys)
                    {
                        f.Item2.Add(index);
                    }
                }
                else
                {
                    _data[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified tuple of key components.
        /// </summary>
        /// <param name="keys">A tuple describing the two components of the composite key.</param>
        /// <returns>The value stored under the provided tuple of keys.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any element of <paramref name="keys"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when retrieving a value for a key combination that has not been stored.</exception>
        public TValue this[(T1 key1, T2 key2) keys]
        {
            get => this[keys.key1, keys.key2];
            set => this[keys.key1, keys.key2] = value;
        }

        /// <summary>
        /// Adds a value associated with the specified two-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="value">The value to associate with the provided composite key.</param>
        /// <exception cref="ArgumentNullException">Thrown when any key component is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a value already exists for the provided key combination.</exception>
        public void Add(T1 key1, T2 key2, TValue value)
        {
            if (Contains(key1, key2)) throw new ArgumentException($"Key already exists ({key1}, {key2})");
            this[key1, key2] = value;
        }

        /// <summary>
        /// Adds a value associated with the specified tuple of key components.
        /// </summary>
        /// <param name="keys">A tuple containing the two key components.</param>
        /// <param name="value">The value to associate with the provided composite key.</param>
        /// <exception cref="ArgumentNullException">Thrown when any element of <paramref name="keys"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a value already exists for the provided key combination.</exception>
        public void Add((T1 key1, T2 key2) keys, TValue value) => Add(keys.key1, keys.key2, value);

        /// <summary>
        /// Removes all entries that include the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove1(T1 key1)
        {
            if (!_indices1.TryGetValue(key1, out var indices1) || indices1.Count == 0)
                return 0;
            foreach (var index in indices1)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices1);
            }
            foreach (var indices in _indices2.Values)
            {
                indices.ExceptWith(indices1);
            }
            var removed = indices1.Count;
            _unusedIndices.UnionWith(indices1);
            indices1.Clear();
            CheckCompact();
            return removed;
        }

        /// <summary>
        /// Removes all entries that include the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component whose associated entries should be removed.</param>
        /// <returns>The number of entries removed for the specified key component.</returns>
        public int Remove2(T2 key2)
        {
            if (!_indices2.TryGetValue(key2, out var indices2) || indices2.Count == 0)
                return 0;
            foreach (var index in indices2)
            {
                var key = ArrayTuple.From(_collapsedKeys, _keys1[index], _keys2[index]);
                _fullIndex.Remove(key);
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(indices2);
            }
            foreach (var indices in _indices1.Values)
            {
                indices.ExceptWith(indices2);
            }
            var removed = indices2.Count;
            _unusedIndices.UnionWith(indices2);
            indices2.Clear();
            CheckCompact();
            return removed;
        }

#if NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0
        /// <summary>
        /// Attempts to retrieve the value associated with the specified two-key combination.
        /// </summary>
        /// <param name="key1">The first component of the composite key.</param>
        /// <param name="key2">The second component of the composite key.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key combination, if found; otherwise, the default value for <typeparamref name="TValue"/>.</param>
        /// <returns><c>true</c> when the key combination exists; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(T1 key1, T2 key2, out TValue value)
#else
        /// <inheritdoc cref="TryGetValue(T1, T2, out TValue)"/>
        public bool TryGetValue(T1 key1, T2 key2, out TValue? value)
#endif
        {
            if (!_fullIndex.TryGetValue(ArrayTuple.From(_collapsedKeys, key1, key2), out var index))
            {
                value = default;
                return false;
            }
            value = _data[index];
            return true;
        }

        /// <summary>
        /// Creates a one-dimensional slice scoped to the provided first key component.
        /// </summary>
        /// <param name="key1">The first key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T2, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T2, TValue> Slice1(T1 key1)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            var ind = _indices1[key1];
            if (ind.Count == 0) throw new ArgumentException($"key {key1} not found", nameof(key1));
            return new MultiKeyDictionary<T2, TValue>(
                _indices2,
                _fullIndex,
                _data, _keys2,
                _collapsedKeys.Concat(new[] { ((object)key1, ind, 0) }).ToList(), _unusedIndices);
        }

        /// <summary>
        /// Creates a one-dimensional slice scoped to the provided second key component.
        /// </summary>
        /// <param name="key2">The second key component that will remain fixed in the resulting slice.</param>
        /// <returns>A new <see cref="MultiKeyDictionary{T1, TValue}"/> exposing values for the matching combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided key component is not present in the dictionary.</exception>
        public MultiKeyDictionary<T1, TValue> Slice2(T2 key2)
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            var ind = _indices2[key2];
            if (ind.Count == 0) throw new ArgumentException($"key {key2} not found", nameof(key2));
            return new MultiKeyDictionary<T1, TValue>(
                _indices1,
                _fullIndex,
                _data, _keys1,
                _collapsedKeys.Concat(new[] { ((object)key2, ind, 1) }).ToList(), _unusedIndices);
        }
        /// <summary>
        /// Determines whether any value exists that includes the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key1"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        public bool ContainsKey1(T1 key1)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            if (!_indices1.TryGetValue(key1, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }
        /// <summary>
        /// Determines whether any value exists that includes the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to check.</param>
        /// <returns><c>true</c> if at least one value is associated with <paramref name="key2"/> within the current scope; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        public bool ContainsKey2(T2 key2)
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            if (!_indices2.TryGetValue(key2, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified first key component.
        /// </summary>
        /// <param name="key1">The first key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        public bool TrySlice1(T1 key1, out MultiKeyDictionary<T2, TValue> values)
#else
        /// <inheritdoc cref="TrySlice1(T1, out MultiKeyDictionary{T2, TValue})"/>
        public bool TrySlice1(T1 key1, out MultiKeyDictionary<T2, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            if (_indices1.TryGetValue(key1, out var indices) && indices.Count > 0)
            {
                values = Slice1(key1);
                return true;
            }
            values = default;
            return false;
        }

#if (NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0)
        /// <summary>
        /// Attempts to create a slice constrained to the specified second key component.
        /// </summary>
        /// <param name="key2">The second key component to constrain.</param>
        /// <param name="values">When this method returns, contains the slice representing remaining key components when the key exists; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the slice could be created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key2"/> is <c>null</c>.</exception>
        public bool TrySlice2(T2 key2, out MultiKeyDictionary<T1, TValue> values)
#else
        /// <inheritdoc cref="TrySlice2(T2, out MultiKeyDictionary{T1, TValue})"/>
        public bool TrySlice2(T2 key2, out MultiKeyDictionary<T1, TValue>? values)
#endif
        {
            ExceptionHandling.ThrowIfNull(key2, nameof(key2));
            if (_indices2.TryGetValue(key2, out var indices) && indices.Count > 0)
            {
                values = Slice2(key2);
                return true;
            }
            values = default;
            return false;
        }

        /// <summary>
        /// Enumerates the stored entries as tuples containing both key components and the associated value.
        /// </summary>
        /// <returns>An iterator that yields tuples of each composite key and its corresponding value.</returns>
        public IEnumerable<(T1, T2, TValue)> Enumerate()
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices != null)
            {
                foreach (var i in collapsedIndices)
                {
                    yield return ((T1)_keys1[i], (T2)_keys2[i], _data[i]);
                }
            }
            else
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (!_unusedIndices.Contains(i))
                        yield return ((T1)_keys1[i], (T2)_keys2[i], _data[i]);
                }
            }
        }

        /// <summary>
        /// Materializes the dictionary into a standard <see cref="Dictionary{TKey, TValue}"/> using tuple-based composite keys.
        /// </summary>
        /// <returns>A dictionary containing every stored key combination and its associated value.</returns>
        public Dictionary<(T1, T2), TValue> ToDictionary() => Enumerate().ToDictionary(x => (x.Item1, x.Item2), x => x.Item3);
    }

    /// <summary>
    /// Represents a dictionary that indexes values by a single key component with optional slicing support.
    /// </summary>
    /// <typeparam name="T1">The type of the key component.</typeparam>
    /// <typeparam name="TValue">The type of values stored in the dictionary.</typeparam>
    public class MultiKeyDictionary<T1, TValue> : IEnumerable<TValue>
#if !(NET472 || NET481 || NETSTANDARD2_0)
        where T1 : notnull
#endif
    {
        private IDictionary<T1, ISet<int>> _indices1;
        private IDictionary<ArrayTuple, int> _fullIndex;
        private IList<TValue> _data;
        private IList<T1> _keys1;
        private IList<(object, ISet<int>, int)> _collapsedKeys;
        private ISet<int> _unusedIndices;

        /// <summary>
        /// Removals do not actually remove data from the table, but just delete the index.
        /// If a lot of Remove operations are performed, the table will become very sparsely populated with actual data.
        /// This setting controls the absolute number of unused indices that will trigger a compaction.
        /// Both this and the relative threshold must be met for a compaction to occur.
        /// </summary>
        /// <value>The number of unused indices before a compaction is performed.</value>
        public int CompactingAbsoluteThreshold { get; set; } = 1000;
        /// <summary>
        /// Removals do not actually remove data from the table, but just delete the index.
        /// If a lot of Remove operations are performed, the table will become very sparsely populated with actual data.
        /// This setting controls the relative number of unused indices that will trigger a compaction.
        /// Both this and the absolute threshold must be met for a compaction to occur.
        /// </summary>
        /// <value>The relative number of unused indices (relative to the full size of the array) before a compaction is performed.</value>
        public double CompactingRelativeThreshold { get; set; } = 0.01;


        protected internal MultiKeyDictionary(IDictionary<T1, ISet<int>> indices1,
            IDictionary<ArrayTuple, int> fullIndex, IList<TValue> data,
            IList<T1> keys1,
            IList<(object, ISet<int>, int)> collapsedKeys, ISet<int> unusedIndices)
        {
            _indices1 = indices1;
            _fullIndex = fullIndex;
            _data = data;
            _keys1 = keys1;
            _collapsedKeys = collapsedKeys;
            _unusedIndices = unusedIndices;
        }
        /// <summary>
        /// Initializes an empty dictionary keyed by a single component.
        /// </summary>
        public MultiKeyDictionary() : this(new KeyValuePair<T1, TValue>[0]) { }

        /// <summary>
        /// Initializes the dictionary with existing entries provided as key/value pairs.
        /// </summary>
        /// <param name="data">An enumerable of key/value pairs used to populate the dictionary.</param>
        public MultiKeyDictionary(IEnumerable<KeyValuePair<T1, TValue>> data)
            : this(data.Select(x => (x.Key, x.Value))) { }

        /// <summary>
        /// Initializes the dictionary with existing entries specified as tuples of key and value.
        /// </summary>
        /// <param name="data">An enumerable of tuples that provide the key followed by the associated value.</param>
        public MultiKeyDictionary(IEnumerable<(T1, TValue)> data)
        {
            _indices1 = new Dictionary<T1, ISet<int>>();
            _fullIndex = new Dictionary<ArrayTuple, int>(100);
            _data = new List<TValue>(100);
            _keys1 = new List<T1>(100);
            _collapsedKeys = new List<(object, ISet<int>, int)>();
            _unusedIndices = new HashSet<int>();

            var idx = -1;
            foreach (var d in data)
            {
                ++idx;
                var (key1, val) = d;
                _data.Add(val);
                _fullIndex[ArrayTuple.From(key1)] = idx;
                _keys1.Add(key1);
                if (!_indices1.TryGetValue(key1, out var ind1))
                {
                    ind1 = new HashSet<int>();
                    _indices1[key1] = ind1;
                }
                ind1.Add(idx);
            }
        }
        /// <summary>
        /// Gets the number of stored values, accounting for unused indices and any active slice.
        /// </summary>
        /// <returns>The number of accessible entries in the dictionary or slice.</returns>
        public int Count => GetCollapsedIndexsetOrDefault()?.Count ?? (_data.Count - _unusedIndices.Count);

        /// <summary>
        /// Sets *all* values in the table (respectively the slice) to the given value
        /// </summary>
        /// <param name="value"></param>
        public void Set(TValue value)
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices == null)
            {
                // set all values
                for (var i = 0; i < _data.Count; i++)
                {
                    // This method also overrides unused indices *shrug*
                    _data[i] = value;
                }
            }
            else
            {
                // set those values that are in the slice
                foreach (var index in collapsedIndices)
                    _data[index] = value;
            }
        }

        /// <summary>
        /// Removes the entry associated with the specified key.
        /// </summary>
        /// <param name="key1">The key whose associated value should be removed.</param>
        /// <returns><c>true</c> if the entry existed and was removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        public bool Remove(T1 key1)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));

            var key = ArrayTuple.From(_collapsedKeys, key1);
            if (!_fullIndex.TryGetValue(key, out var index))
            {
                return false;
            }
            _fullIndex.Remove(key);
            _unusedIndices.Add(index);
            _data[index] = default;
            _keys1[index] = default;
            for (var i = 0; i < _collapsedKeys.Count; i++)
            {
                _collapsedKeys[i].Item2.Remove(index); // we don't need to manipulate all indices > index, because we don't remove above
            }
            _indices1[key1].Remove(index);
            CheckCompact();
            return true;
        }

        /// <summary>
        /// Removes all stored keys or, when sliced, the values visible within the active slice.
        /// </summary>
        /// <remarks>
        /// Index structures are updated to reflect removed keys, and unused slots are compacted when necessary.
        /// </remarks>
        public void Clear()
        {
            var collapsed = GetCollapsedIndexsetOrDefault();
            if (collapsed == null)
            {
                _fullIndex.Clear();
                _data.Clear();
                _keys1.Clear();
                _unusedIndices.Clear();
                foreach (var indices in _indices1.Values)
                {
                    indices.Clear();
                }
                foreach (var indices in _collapsedKeys)
                {
                    indices.Item2.Clear();
                }
            }
            else
            {
                foreach (var ind in collapsed)
                {
                    var key = ArrayTuple.From(_collapsedKeys, _keys1[ind]);
                    _fullIndex.Remove(key);
                }
                foreach (var indices in _indices1.Values)
                {
                    indices.ExceptWith(collapsed);
                }
                foreach (var indices in _collapsedKeys)
                {
                    indices.Item2.ExceptWith(collapsed);
                }
                // declare the indices unused and then compact
                _unusedIndices.UnionWith(collapsed);
                Compact();
            }
        }

        protected void CheckCompact()
        {
            if (_unusedIndices.Count > CompactingAbsoluteThreshold
                && _unusedIndices.Count > _data.Count * CompactingRelativeThreshold)
                Compact();
        }

        private void Compact()
        {
            if (_unusedIndices.Count == 0) return;
            var sequence = _unusedIndices.OrderByDescending(x => x).ToList();
            var offsets = new Dictionary<int, int>();
            var offset = 0;
            var offsetIndex = 0;
            for (var i = sequence.Count - 1; i >= 0; i--)
            {
                var index = sequence[i];
                if (offset > 0)
                {
                    for (var o = offsetIndex; o < index; o++)
                    {
                        offsets[o] = offset;
                    }
                }
                offsetIndex = index;
                offset++;
                _data.RemoveAt(sequence[sequence.Count - 1 - i]);
                _keys1.RemoveAt(sequence[sequence.Count - 1 - i]);
            }
            for (var o = offsetIndex; o < _data.Count + offset; o++)
            {
                offsets[o] = offset;
            }
            foreach (var kvp in _fullIndex.ToArray())
            {
                if (offsets.TryGetValue(kvp.Value, out var off))
                {
                    _fullIndex[kvp.Key] = kvp.Value - off;
                }
            }
            foreach (var indices in _collapsedKeys)
            {
                indices.Item2.ExceptWith(_unusedIndices);
                var reindex = indices.Item2.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.Item2.ExceptWith(reindex.Select(x => x.index));
                    indices.Item2.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            foreach (var kvp in _indices1)
            {
                var key = kvp.Key;
                var indices = kvp.Value;
                indices.ExceptWith(_unusedIndices);
                var reindex = indices.Select(x => (index: x, offset: offsets.TryGetValue(x, out var o) ? o : 0)).Where(x => x.offset > 0).ToList();
                if (reindex.Count > 0)
                {
                    indices.ExceptWith(reindex.Select(x => x.index));
                    indices.UnionWith(reindex.Select(x => x.index - x.offset));
                }
            }
            _unusedIndices.Clear();
        }

#if NET472 || NET481 || NETSTANDARD2_0
        protected ISet<int> GetCollapsedIndexsetOrDefault()
        {
            HashSet<int> collapsedIndices = null;
#else
        protected ISet<int>? GetCollapsedIndexsetOrDefault()
        {
            HashSet<int>? collapsedIndices = null;
#endif
            if (_collapsedKeys.Count > 0)
            {
                foreach (var collapsedSet in _collapsedKeys.Select(x => x.Item2).OrderBy(x => x.Count))
                {
                    if (collapsedIndices == null)
                        collapsedIndices = new HashSet<int>(collapsedSet);
                    else collapsedIndices.IntersectWith(collapsedSet);
                }
            }
            return collapsedIndices;
        }
        /// <summary>
        /// Enumerates values currently visible within the dictionary, respecting any active slice.
        /// </summary>
        /// <returns>An iterator that yields values for each stored key combination in scope.</returns>
        public IEnumerable<TValue> EnumerateValues()
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices == null)
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (!_unusedIndices.Contains(i))
                        yield return _data[i];
                }
            }
            else
            {
                foreach (var index in collapsedIndices)
                {
                    yield return _data[index];
                }
            }
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return EnumerateValues().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumerateValues().GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key1">The key whose value should be retrieved or assigned.</param>
        /// <returns>The value stored under the provided key.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when retrieving a value for a key that has not been stored.</exception>
        public TValue this[T1 key1]
        {
            get
            {
                ExceptionHandling.ThrowIfNull(key1, nameof(key1));
                if (!_fullIndex.TryGetValue(ArrayTuple.From(_collapsedKeys, key1), out var index))
                {
                    throw new ArgumentException($"Key combination {(key1)} not found");
                }

                return _data[index];
            }
            set
            {
                ExceptionHandling.ThrowIfNull(key1, nameof(key1));

                var tupKey = ArrayTuple.From(_collapsedKeys, key1);
                if (!_fullIndex.TryGetValue(tupKey, out var index))
                    index = -1;
                if (index < 0)
                {
                    if (!_indices1.TryGetValue(key1, out var indices1))
                    {
                        indices1 = new HashSet<int>();
                        _indices1[key1] = indices1;
                    }
                    if (_unusedIndices.Count > 0)
                    {
                        // reuse an unused index
                        index = _unusedIndices.First();
                        _unusedIndices.Remove(index);
                        _data[index] = value;
                        _keys1[index] = key1;
                    }
                    else
                    {
                        // make an insert at the end
                        index = _data.Count;
                        _data.Add(value);
                        _keys1.Add(key1);
                    }
                    _fullIndex[tupKey] = index;
                    indices1.Add(index);
                    // the data needs to be added to all collapsed dimensions
                    foreach (var f in _collapsedKeys)
                    {
                        f.Item2.Add(index);
                    }
                }
                else
                {
                    _data[index] = value;
                }
            }
        }

        /// <summary>
        /// Adds a value associated with the specified key.
        /// </summary>
        /// <param name="key1">The key to associate with the value.</param>
        /// <param name="value">The value to store.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when a value already exists for <paramref name="key1"/>.</exception>
        public void Add(T1 key1, TValue value)
        {
            if (Contains(key1)) throw new ArgumentException($"Key already exists ({key1})");
            this[key1] = value;
        }

#if NET472 || NET481 || NETSTANDARD2_1 || NETSTANDARD2_0
        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// </summary>
        /// <param name="key1">The key whose value should be retrieved.</param>
        /// <param name="value">When this method returns, contains the value associated with <paramref name="key1"/>, if found; otherwise, the default value for <typeparamref name="TValue"/>.</param>
        /// <returns><c>true</c> when the key exists; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(T1 key1, out TValue value)
#else
        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// </summary>
        /// <param name="key1">The key whose value should be retrieved.</param>
        /// <param name="value">When this method returns, contains the value associated with <paramref name="key1"/>, if found; otherwise, the default value for <typeparamref name="TValue"/>.</param>
        /// <returns><c>true</c> when the key exists; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(T1 key1, out TValue? value)
#endif
        {
            if (!_fullIndex.TryGetValue(ArrayTuple.From(_collapsedKeys, key1), out var index))
            {
                value = default;
                return false;
            }
            value = _data[index];
            return true;
        }

        /// <summary>
        /// Determines whether a value exists for the specified key.
        /// </summary>
        /// <param name="key1">The key to check.</param>
        /// <returns><c>true</c> if a value is associated with <paramref name="key1"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key1"/> is <c>null</c>.</exception>
        public bool Contains(T1 key1)
        {
            ExceptionHandling.ThrowIfNull(key1, nameof(key1));
            if (!_indices1.TryGetValue(key1, out var ind) || ind.Count == 0)
                return false;

            var indices = GetCollapsedIndexsetOrDefault();
            if (indices == null)
            {
                return true; // otherwise, we'd have returned false above
            }
            else
            {
                indices.IntersectWith(ind);
            }
            if (indices.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// Determines whether a value exists for the specified key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><c>true</c> if a value is associated with <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(T1 key) => Contains(key);

        /// <summary>
        /// Enumerates the stored entries as tuples containing the key and associated value.
        /// </summary>
        /// <returns>An iterator that yields tuples of each key and its corresponding value.</returns>
        public IEnumerable<(T1, TValue)> Enumerate()
        {
            var collapsedIndices = GetCollapsedIndexsetOrDefault();
            if (collapsedIndices != null)
            {
                foreach (var i in collapsedIndices)
                {
                    yield return ((T1)_keys1[i], _data[i]);
                }
            }
            else
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    if (!_unusedIndices.Contains(i))
                        yield return ((T1)_keys1[i], _data[i]);
                }
            }
        }

        /// <summary>
        /// Materializes the dictionary into a standard <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A dictionary containing each stored key and its associated value.</returns>
        public Dictionary<T1, TValue> ToDictionary() => Enumerate().ToDictionary(x => x.Item1, x => x.Item2);
    }

    internal static class ExceptionHandling
    {
#if NETSTANDARD2_1 || NET472 || NET481 || NETSTANDARD2_0
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> when the supplied argument is <c>null</c>.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="paramName">The name of the parameter being validated.</param>
        public static void ThrowIfNull(object argument, string paramName = null)
        {
            if (argument == null)
                throw new ArgumentNullException(paramName ?? string.Empty);
        }
#else
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> when the supplied argument is <c>null</c>.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <param name="paramName">The name of the parameter being validated.</param>
        public static void ThrowIfNull(object? argument, string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);
        }
#endif
    }
}
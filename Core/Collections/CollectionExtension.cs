//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 18-11-2023
// Description      : v1.7.1
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.       
//----------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VisionNet.Core.Collections
{
    /// <summary>
    /// Provides extension methods for collections with additional functionality.
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// Finds the index of the first item that satisfies the given predicate.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="items">The enumerable collection of items.</param>
        /// <param name="predicate">The predicate used to find the item.</param>
        /// <returns>The index of the found item, or -1 if not found.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            // GUARD: Ensure required arguments are provided.
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            bool found = false;
            int index = 0;
            foreach (var item in items)
            {
                if (found = predicate(item))
                    break;
                index++;
            }

            if (found)
                return index;
            else
                return -1;
        }

        /// <summary>
        /// Groups the items in the collection into pairs of two items. 
        /// Throws an exception if the number of items is odd.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="items">The collection to group in pairs.</param>
        /// <returns>A collection of tuples, each containing two items from the original collection.</returns>
        /// <exception cref="ArgumentException">Thrown when the number of items in the collection is odd.</exception>
        public static IEnumerable<Tuple<T, T>> GroupInPairs<T>(this IEnumerable<T> items)
        {
            // GUARD: Ensure required arguments are provided.
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (items.Count() % 2 != 0)
                throw new ArgumentException("Items count must be an even number");

            var points = items.SelectMany((n, i) => new[] { new { index = i, value = n } });
            var even = points.Where(n => n.index % 2 == 0);
            var odd = points.Where(n => n.index % 2 != 0);

            return even.Zip(odd, (a, b) => new Tuple<T, T>(a.value, b.value));
        }

        /// <summary>
        /// Generates all possible permutations of the given collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="values">The collection to permute.</param>
        /// <returns>A collection of all possible permutations of the items in the original collection.</returns>
        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> values) => values.SelectMany(x => Permute(new[] { new[] { x } }, values, values.Count() - 1));

        /// <summary>
        /// Generates a specified number of permutations of the given collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="values">The collection to permute.</param>
        /// <param name="permutations">The number of permutations to generate.</param>
        /// <returns>A collection of permutations of the items in the original collection.</returns>
        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> values, int permutations) => values.SelectMany(x => Permute(new[] { new[] { x } }, values, permutations - 1));

        private static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<IEnumerable<T>> current, IEnumerable<T> values, int count) => (count == 1) ? Permute(current, values) : Permute(Permute(current, values), values, --count);

        private static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<IEnumerable<T>> current, IEnumerable<T> values) => current.SelectMany(x => values.Select(y => x.Concat(new[] { y })));

        /// <summary>
        /// Performs a full outer join on two sequences based on the specified key selectors and projection function.
        /// </summary>
        /// <typeparam name="TLeft">The type of elements in the left sequence.</typeparam>
        /// <typeparam name="TRight">The type of elements in the right sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key used for joining.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the projection function.</typeparam>
        /// <param name="left">The left sequence.</param>
        /// <param name="right">The right sequence.</param>
        /// <param name="leftKeySelector">The key selector for the left sequence.</param>
        /// <param name="rightKeySelector">The key selector for the right sequence.</param>
        /// <param name="projection">The projection function that produces a result based on elements from both sequences.</param>
        /// <param name="leftDefault">The default value to use for missing elements in the left sequence.</param>
        /// <param name="rightDefault">The default value to use for missing elements in the right sequence.</param>
        /// <param name="keyComparer">An optional custom equality comparer for comparing keys.</param>
        /// <returns>A sequence of projection results for each key that is present in either the left or right sequence.</returns>
        public static IEnumerable<TResult> FullOuterJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, TKey, TResult> projection, TLeft leftDefault = default(TLeft), TRight rightDefault = default(TRight), IEqualityComparer<TKey> keyComparer = null)
        {
            keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            ILookup<TKey, TLeft> leftLookup = left.ToLookup(leftKeySelector, keyComparer);
            ILookup<TKey, TRight> rightLookup = right.ToLookup(rightKeySelector, keyComparer);

            HashSet<TKey> keys = new HashSet<TKey>(leftLookup.Select(p => p.Key), keyComparer);
            keys.UnionWith(rightLookup.Select(p => p.Key));

            return from key in keys
                   from leftValue in leftLookup[key].DefaultIfEmpty(leftDefault)
                   from rightValue in rightLookup[key].DefaultIfEmpty(rightDefault)
                   select projection(leftValue, rightValue, key);
        }

        /// <summary>
        /// Groups elements in the collection into sub-sequences based on a condition that compares consecutive elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="seq">The sequence to group.</param>
        /// <param name="condition">The condition used to determine whether consecutive elements belong to the same group.</param>
        /// <returns>A sequence of sub-sequences where each sub-sequence contains elements that satisfy the given condition.</returns>
        public static IEnumerable<IEnumerable<T>> GroupWhile<T>(this IEnumerable<T> seq, Func<T, T, bool> condition)
        {
            // GUARD: Ensure required arguments are provided.
            if (seq == null)
                throw new ArgumentNullException(nameof(seq));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            using (IEnumerator<T> enumerator = seq.GetEnumerator())
            {
                if (enumerator.MoveNext() == false)
                    yield break;

                T prev = enumerator.Current;
                List<T> list = new List<T>() { prev };

                while (enumerator.MoveNext())
                {
                    T item = enumerator.Current;

                    if (condition(prev, item) == false)
                    {
                        yield return list;
                        list = new List<T>();
                    }
                    list.Add(item);
                    prev = item;
                }

                yield return list;
            }
        }

        /// <summary>
        /// Computes the standard deviation of a sequence of double values.
        /// </summary>
        /// <param name="values">The sequence of values.</param>
        /// <returns>The standard deviation of the values in the sequence.</returns>
        public static double StdDev(this IEnumerable<double> values)
        {
            double ret = 0;

            if (values.Count() > 0)
            {
                // Compute the Average
                double avg = values.Average();

                // Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => System.Math.Pow(d - avg, 2));

                // Put it all together
                ret = System.Math.Sqrt(sum / ((double)values.Count() - 1.0));
            }
            return ret;
        }

        /// <summary>
        /// Checks if the sequence is null or empty.
        /// </summary>
        /// <param name="this">The sequence to check.</param>
        /// <returns>True if the sequence is null or empty, otherwise false.</returns>
        public static bool IsNullOrEmpty(this IEnumerable @this)
        {
            return @this == null || @this.GetEnumerator().MoveNext() == false;
        }

        /// <summary>
        /// Executes the given action on each element of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="enumeration">The sequence to iterate over.</param>
        /// <param name="action">The action to execute on each element.</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            // GUARD: Ensure required arguments are provided.
            if (enumeration == null)
                throw new ArgumentNullException(nameof(enumeration));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Determines whether the supplied object represents a generic <see cref="IList"/> whose element type is assignable to the expected type.
        /// </summary>
        /// <param name="obj">The instance to inspect for <see cref="IList"/> compatibility; may be <c>null</c> when the check should fail.</param>
        /// <param name="expectedElementType">The element type that valid list implementations must expose through their generic argument.</param>
        /// <returns><c>true</c> when <paramref name="obj"/> implements <see cref="IList{T}"/> for an element type that can be assigned to <paramref name="expectedElementType"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="expectedElementType"/> is <c>null</c>, because the runtime cannot evaluate assignability.</exception>
        public static bool IsListOf(this object obj, Type expectedElementType)
        {
            // If the instance is any IList (non-generic or generic), proceed to inspect generic type arguments.
            if (obj is IList)
            {
                var objType = obj.GetType();

                // Only generic types can expose ILists with element type metadata for assignability checks.
                if (objType.IsGenericType)
                {
                    // Locate the IList<T> interface among the implemented interfaces so its type arguments can be inspected.
                    var iListInterface = objType.GetInterfaces()
                        .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));

                    if (iListInterface != null && iListInterface.Count() == 1)
                    {
                        // Compare the discovered element type with the requested expected type to confirm compatibility.
                        var elementType = iListInterface.FirstOrDefault()?.GetGenericArguments()[0];
                        return expectedElementType.IsAssignableFrom(elementType);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the sequence contains an element that is equal to the specified value using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">The element type stored in the sequence.</typeparam>
        /// <param name="sequence">The sequence to search; must not be <c>null</c>.</param>
        /// <param name="value">The value to locate using <see cref="EqualityComparer{T}.Default"/>.</param>
        /// <returns><c>true</c> when at least one element matches <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="sequence"/> is <c>null</c>, because enumeration cannot be performed.</exception>
        public static bool Contains<T>(this IEnumerable<T> sequence, T value)
        {
            IEqualityComparer<T> @default = EqualityComparer<T>.Default;
            foreach (T item in sequence)
            {
                if (@default.Equals(item, value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the non-generic sequence contains an element equal to the specified value using the default object comparer.
        /// </summary>
        /// <param name="sequence">The sequence to search; must not be <c>null</c>.</param>
        /// <param name="value">The value to locate using <see cref="Comparer{T}.Default"/> for <see cref="object"/> instances.</param>
        /// <returns><c>true</c> when an element compares equal to <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="sequence"/> is <c>null</c>, because enumeration cannot be performed.</exception>
        /// <exception cref="ArgumentException">Thrown when elements of <paramref name="sequence"/> do not support comparison with <paramref name="value"/> via <see cref="IComparable"/>.</exception>
        public static bool Contains(this IEnumerable sequence, object value)
        {
            Comparer<object> @default = Comparer<object>.Default;
            foreach (object item in sequence)
            {
                if (@default.Compare(item, value) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the sequence contains any element that is equal to at least one value from the provided candidate set.
        /// </summary>
        /// <typeparam name="T">The element type stored in the sequence and candidate set.</typeparam>
        /// <param name="sequence">The sequence to search for matches; must not be <c>null</c>.</param>
        /// <param name="values">The candidate values to look for; may be <c>null</c> or empty to short-circuit the check.</param>
        /// <returns><c>true</c> when <paramref name="sequence"/> contains any element present in <paramref name="values"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="sequence"/> is <c>null</c>, because enumeration cannot be performed.</exception>
        public static bool ContainsAny<T>(this IEnumerable<T> sequence, IEnumerable<T> values)
        {
            if (IsSequenceNullOrEmpty(values))
            {
                return false;
            }

            if (IsSequenceNullOrEmpty(sequence))
            {
                return false;
            }

            bool sequenceContainsNull;
            Dictionary<T, byte> dictionary = ConvertToSet(sequence, out sequenceContainsNull);
            foreach (T value in values)
            {
                if (value == null)
                {
                    if (sequenceContainsNull)
                    {
                        return true;
                    }
                }
                else if (dictionary.ContainsKey(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the non-generic sequence contains any element that equals one of the supplied candidate values using object equality semantics.
        /// </summary>
        /// <param name="sequence">The sequence to inspect; must not be <c>null</c>.</param>
        /// <param name="values">The candidate values to look for; may be <c>null</c> or empty to short-circuit the check.</param>
        /// <returns><c>true</c> when <paramref name="sequence"/> contains any of the elements in <paramref name="values"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="sequence"/> is <c>null</c>, because enumeration cannot be performed.</exception>
        public static bool ContainsAny(this IEnumerable sequence, IEnumerable values)
        {
            if (IsSequenceNullOrEmpty(values))
            {
                return false;
            }

            if (IsSequenceNullOrEmpty(sequence))
            {
                return false;
            }

            bool sequenceContainsNull;
            Dictionary<object, byte> dictionary = ConvertToSet(sequence, out sequenceContainsNull);
            foreach (object value in values)
            {
                if (value == null)
                {
                    if (sequenceContainsNull)
                    {
                        return true;
                    }
                }
                else if (dictionary.ContainsKey(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> sequence, IEnumerable<T> values)
        {
            if (IsSequenceNullOrEmpty(values))
            {
                return true;
            }

            if (IsSequenceNullOrEmpty(sequence))
            {
                return false;
            }

            bool sequenceContainsNull;
            Dictionary<T, byte> dictionary = ConvertToSet(sequence, out sequenceContainsNull);
            foreach (T value in values)
            {
                if (value == null)
                {
                    if (!sequenceContainsNull)
                    {
                        return false;
                    }
                }
                else if (!dictionary.ContainsKey(value))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ContainsAll(this IEnumerable sequence, IEnumerable values)
        {
            if (IsSequenceNullOrEmpty(values))
            {
                return true;
            }

            if (IsSequenceNullOrEmpty(sequence))
            {
                return false;
            }

            bool sequenceContainsNull;
            Dictionary<object, byte> dictionary = ConvertToSet(sequence, out sequenceContainsNull);
            foreach (object value in values)
            {
                if (value == null)
                {
                    if (!sequenceContainsNull)
                    {
                        return false;
                    }
                }
                else if (!dictionary.ContainsKey(value))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool SequenceHasLength(this IEnumerable sequence, int numberOfElements)
        {
            if (sequence == null)
            {
                return numberOfElements == 0;
            }

            ICollection collection = sequence as ICollection;
            if (collection != null)
            {
                return collection.Count == numberOfElements;
            }

            IEnumerator enumerator = sequence.GetEnumerator();
            try
            {
                int num = 0;
                while (enumerator.MoveNext())
                {
                    num++;
                    if (num > numberOfElements)
                    {
                        return false;
                    }
                }

                return num == numberOfElements;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public static bool IsSequenceNullOrEmpty<TSource>(this IEnumerable<TSource> sequence)
        {
            if (sequence == null)
            {
                return true;
            }

            ICollection<TSource> collection = sequence as ICollection<TSource>;
            if (collection != null)
            {
                return collection.Count == 0;
            }

            return IsSequenceNullOrEmpty((IEnumerable)sequence);
        }

        public static bool IsSequenceNullOrEmpty(this IEnumerable sequence)
        {
            if (sequence == null)
            {
                return true;
            }

            ICollection collection = sequence as ICollection;
            if (collection != null)
            {
                return collection.Count == 0;
            }

            return IsEnumerableEmpty(sequence);
        }

        public static bool SequenceIsShorterThan(this IEnumerable sequence, int numberOfElements)
        {
            if (sequence == null)
            {
                return 0 < numberOfElements;
            }

            ICollection collection = sequence as ICollection;
            if (collection != null)
            {
                return collection.Count < numberOfElements;
            }

            IEnumerator enumerator = sequence.GetEnumerator();
            try
            {
                int num = 0;
                while (enumerator.MoveNext())
                {
                    num++;
                    if (num >= numberOfElements)
                    {
                        return false;
                    }
                }

                return num < numberOfElements;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public static bool SequenceIsShorterOrEqual(this IEnumerable sequence, int numberOfElements)
        {
            if (sequence == null)
            {
                return 0 <= numberOfElements;
            }

            ICollection collection = sequence as ICollection;
            if (collection != null)
            {
                return collection.Count <= numberOfElements;
            }

            IEnumerator enumerator = sequence.GetEnumerator();
            try
            {
                int num = 0;
                while (enumerator.MoveNext())
                {
                    num++;
                    if (num > numberOfElements)
                    {
                        return false;
                    }
                }

                return num <= numberOfElements;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public static bool SequenceIsLongerThan(this IEnumerable sequence, int numberOfElements)
        {
            if (sequence == null)
            {
                return 0 > numberOfElements;
            }

            ICollection collection = sequence as ICollection;
            if (collection != null)
            {
                return collection.Count > numberOfElements;
            }

            IEnumerator enumerator = sequence.GetEnumerator();
            try
            {
                int num = 0;
                while (enumerator.MoveNext())
                {
                    num++;
                    if (num > numberOfElements)
                    {
                        return true;
                    }
                }

                return num > numberOfElements;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public static bool SequenceIsLongerOrEqual(this IEnumerable sequence, int numberOfElements)
        {
            if (sequence == null)
            {
                return 0 >= numberOfElements;
            }

            ICollection collection = sequence as ICollection;
            if (collection != null)
            {
                return collection.Count >= numberOfElements;
            }

            IEnumerator enumerator = sequence.GetEnumerator();
            try
            {
                int num = 0;
                while (enumerator.MoveNext())
                {
                    num++;
                    if (num >= numberOfElements)
                    {
                        return true;
                    }
                }

                return num >= numberOfElements;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        public static int GetLength(this IEnumerable sequence)
        {
            ICollection collection = sequence as ICollection;
            if (collection != null)
            {
                return collection.Count;
            }

            IEnumerator enumerator = sequence.GetEnumerator();
            try
            {
                int num = 0;
                while (enumerator.MoveNext())
                {
                    num++;
                }

                return num;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private static bool IsEnumerableEmpty(this IEnumerable sequence)
        {
            IEnumerator enumerator = sequence.GetEnumerator();
            try
            {
                return !enumerator.MoveNext();
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private static Dictionary<T, byte> ConvertToSet<T>(this IEnumerable<T> sequence, out bool sequenceContainsNull)
        {
            sequenceContainsNull = false;
            Dictionary<T, byte> dictionary = new Dictionary<T, byte>(DetermineInitialCapacity(sequence));
            foreach (T item in sequence)
            {
                if (item != null)
                {
                    dictionary[item] = 0;
                }
                else
                {
                    sequenceContainsNull = true;
                }
            }

            return dictionary;
        }

        private static int DetermineInitialCapacity<T>(this IEnumerable<T> sequence)
        {
            return (sequence as ICollection<T>)?.Count ?? 0;
        }

        private static Dictionary<object, byte> ConvertToSet(this IEnumerable sequence, out bool sequenceContainsNull)
        {
            sequenceContainsNull = false;
            Dictionary<object, byte> dictionary = new Dictionary<object, byte>(DetermineInitialCapacity(sequence));
            foreach (object item in sequence)
            {
                if (item != null)
                {
                    dictionary[item] = 0;
                }
                else
                {
                    sequenceContainsNull = true;
                }
            }

            return dictionary;
        }

        private static int DetermineInitialCapacity(this IEnumerable sequence)
        {
            return (sequence as ICollection)?.Count ?? 0;
        }
    }

}
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
            T prev = seq.First();
            List<T> list = new List<T>() { prev };

            foreach (T item in seq.Skip(1))
            {
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
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// The is generic i list of a type
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="expectedElementType">The expected element type.</param>
        /// <returns>The result.</returns>
        public static bool IsListOf(this object obj, Type expectedElementType)
        {
            // Verifica si el objeto es un IList. Esto también será verdadero para IList<T>.
            if (obj is IList)
            {
                var objType = obj.GetType();

                // Verifica si el objeto es un tipo genérico.
                if (objType.IsGenericType)
                {
                    // Obtiene la interfaz IList<T> del objeto, si existe.
                    var iListInterface = objType.GetInterfaces()
                        .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));

                    if (iListInterface != null && iListInterface.Count() == 1)
                    {
                        // Comprueba si el tipo esperado es asignable desde el tipo de los elementos en IList<T>.
                        var elementType = iListInterface.FirstOrDefault()?.GetGenericArguments()[0];
                        return expectedElementType.IsAssignableFrom(elementType);
                    }
                }
            }

            return false;
        }

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

        /// <summary>
        /// Determines whether <paramref name="sequence"/> supplies every element required by <paramref name="values"/>.
        /// Handles <see langword="null"/> or empty <paramref name="values"/> as trivially satisfied and treats a
        /// <see langword="null"/> <paramref name="sequence"/> as empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequences.</typeparam>
        /// <param name="sequence">
        /// The source sequence to inspect; may be <see langword="null"/>, in which case the method returns
        /// <see langword="false"/> unless <paramref name="values"/> is <see langword="null"/> or empty.
        /// </param>
        /// <param name="values">
        /// The values that must be present; may be <see langword="null"/> or empty, which immediately yields
        /// <see langword="true"/>. <see langword="null"/> entries require the source sequence to contain at least one
        /// <see langword="null"/> item.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when every value (respecting <see langword="null"/> semantics) exists in
        /// <paramref name="sequence"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while enumerating <paramref name="sequence"/> or <paramref name="values"/> is
        /// propagated to the caller.
        /// </exception>
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

        /// <summary>
        /// Determines whether <paramref name="sequence"/> supplies every element required by <paramref name="values"/>,
        /// using non-generic comparisons.
        /// Handles <see langword="null"/> or empty <paramref name="values"/> as trivially satisfied and treats a
        /// <see langword="null"/> <paramref name="sequence"/> as empty.
        /// </summary>
        /// <param name="sequence">
        /// The source sequence to inspect; may be <see langword="null"/>, in which case the method returns
        /// <see langword="false"/> unless <paramref name="values"/> is <see langword="null"/> or empty.
        /// </param>
        /// <param name="values">
        /// The values that must be present; may be <see langword="null"/> or empty, which immediately yields
        /// <see langword="true"/>. <see langword="null"/> entries require the source sequence to contain at least one
        /// <see langword="null"/> item.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when every value (respecting <see langword="null"/> semantics) exists in
        /// <paramref name="sequence"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while enumerating <paramref name="sequence"/> or <paramref name="values"/> is
        /// propagated to the caller.
        /// </exception>
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

        /// <summary>
        /// Determines whether <paramref name="sequence"/> exposes exactly <paramref name="numberOfElements"/> items,
        /// treating a <see langword="null"/> sequence as empty.
        /// </summary>
        /// <param name="sequence">
        /// The sequence to inspect; may be <see langword="null"/>, which is treated as having zero elements. Non-null
        /// sequences are fully enumerated when they do not implement <see cref="ICollection"/>.
        /// </param>
        /// <param name="numberOfElements">The expected length to compare against. Negative values are permitted.</param>
        /// <returns>
        /// <see langword="true"/> when the sequence has exactly <paramref name="numberOfElements"/> items; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while enumerating <paramref name="sequence"/> is propagated to the caller.
        /// </exception>
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

        /// <summary>
        /// Determines whether <paramref name="sequence"/> is <see langword="null"/> or exposes no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
        /// <param name="sequence">
        /// The sequence to inspect; may be <see langword="null"/>. When the sequence does not implement
        /// <see cref="ICollection{T}"/>, it is enumerated once.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when <paramref name="sequence"/> is <see langword="null"/> or has no items;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while enumerating <paramref name="sequence"/> is propagated to the caller.
        /// </exception>
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

        /// <summary>
        /// Determines whether <paramref name="sequence"/> is <see langword="null"/> or exposes no elements.
        /// </summary>
        /// <param name="sequence">
        /// The sequence to inspect; may be <see langword="null"/>. When the sequence does not implement
        /// <see cref="ICollection"/>, it is enumerated once.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when <paramref name="sequence"/> is <see langword="null"/> or has no items;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while enumerating <paramref name="sequence"/> is propagated to the caller.
        /// </exception>
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

        /// <summary>
        /// Determines whether <paramref name="sequence"/> contains fewer elements than the specified count, treating a
        /// <see langword="null"/> sequence as empty.
        /// </summary>
        /// <param name="sequence">
        /// The sequence to inspect; may be <see langword="null"/>, which is treated as having zero elements. Non-null
        /// sequences are partially enumerated until the outcome is known.
        /// </param>
        /// <param name="numberOfElements">The comparison length. Negative values are permitted.</param>
        /// <returns>
        /// <see langword="true"/> when the sequence is shorter than <paramref name="numberOfElements"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while enumerating <paramref name="sequence"/> is propagated to the caller.
        /// </exception>
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

        /// <summary>
        /// Determines whether <paramref name="sequence"/> contains no more than the specified number of elements,
        /// treating a <see langword="null"/> sequence as empty.
        /// </summary>
        /// <param name="sequence">
        /// The sequence to inspect; may be <see langword="null"/>, which is treated as having zero elements. Non-null
        /// sequences are partially enumerated until the outcome is known.
        /// </param>
        /// <param name="numberOfElements">The comparison length. Negative values are permitted.</param>
        /// <returns>
        /// <see langword="true"/> when the sequence is shorter than or equal to <paramref name="numberOfElements"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while enumerating <paramref name="sequence"/> is propagated to the caller.
        /// </exception>
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

        /// <summary>
        /// Determines whether <paramref name="sequence"/> contains more elements than the specified count, treating a
        /// <see langword="null"/> sequence as empty.
        /// </summary>
        /// <param name="sequence">
        /// The sequence to inspect; may be <see langword="null"/>, which is treated as having zero elements. Non-null
        /// sequences are partially enumerated until the outcome is known.
        /// </param>
        /// <param name="numberOfElements">The comparison length. Negative values are permitted.</param>
        /// <returns>
        /// <see langword="true"/> when the sequence is longer than <paramref name="numberOfElements"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while enumerating <paramref name="sequence"/> is propagated to the caller.
        /// </exception>
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

        /// <summary>
        /// Determines whether <paramref name="sequence"/> contains at least the specified number of elements, treating a
        /// <see langword="null"/> sequence as empty.
        /// </summary>
        /// <param name="sequence">
        /// The sequence to inspect; may be <see langword="null"/>, which is treated as having zero elements. Non-null
        /// sequences are partially enumerated until the outcome is known.
        /// </param>
        /// <param name="numberOfElements">The comparison length. Negative values are permitted.</param>
        /// <returns>
        /// <see langword="true"/> when the sequence is longer than or equal to <paramref name="numberOfElements"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while enumerating <paramref name="sequence"/> is propagated to the caller.
        /// </exception>
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

        /// <summary>
        /// Calculates the number of elements exposed by <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">
        /// The sequence to inspect. The value must not be <see langword="null"/> because the implementation directly
        /// obtains an enumerator when <see cref="ICollection"/> is not implemented.
        /// </param>
        /// <returns>The total number of items yielded by <paramref name="sequence"/>.</returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when <paramref name="sequence"/> is <see langword="null"/>, because the implementation calls
        /// <see cref="IEnumerable.GetEnumerator"/> without a prior <see langword="null"/> guard.
        /// </exception>
        /// <exception cref="Exception">
        /// Any exception thrown while enumerating <paramref name="sequence"/> is propagated to the caller.
        /// </exception>
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
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

namespace VisionNet.Core.Maths
{
    /// <summary>
    /// Represents an ordered pairing of a numeric value and its observed count so that consumers can
    /// work with frequency-oriented data structures using standard comparison, hashing, and equality semantics.
    /// The ordering invariant sorts primarily by <see cref="Value"/> and secondarily by <see cref="Count"/>.
    /// </summary>
    public class ValueCountTuple : IComparable, IComparable<ValueCountTuple>, IEquatable<ValueCountTuple>
    {
        /// <summary>
        /// Gets or sets the numeric value tracked by this tuple, typically representing the sample or key
        /// for which an occurrence count is stored. No range or precision constraints are enforced by the class.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the number of times the associated <see cref="Value"/> has been observed. The property accepts
        /// any 32-bit integer, including zero or negative numbers when callers need to represent adjustments.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Initializes a new tuple with the provided numeric value and occurrence count, preserving the ordering invariant
        /// that comparisons evaluate by value first and count second.
        /// </summary>
        /// <param name="value">The numeric sample or key whose frequency is being tracked. Any <see cref="double"/> is accepted.</param>
        /// <param name="count">The occurrence count associated with <paramref name="value"/>. Any <see cref="int"/> value is accepted.</param>
        public ValueCountTuple(double value, int count)
        {
            Value = value;
            Count = count;
        }

        /// <summary>
        /// Compares this instance with another object to determine relative ordering, defaulting unmatched or null instances
        /// to the end of the ordering while preserving the value-then-count comparison semantics.
        /// </summary>
        /// <param name="obj">The object to compare with this tuple. Non-<see cref="ValueCountTuple"/> instances are treated as null.</param>
        /// <returns>A signed integer indicating ordering: zero when equal, positive when this instance should follow <paramref name="obj"/>, and negative when it should precede.</returns>
        public int CompareTo(object obj)
        {
            if (ReferenceEquals(this, obj)) return 0;
            if (ReferenceEquals(null, obj)) return 1;

            return CompareTo(obj as ValueCountTuple);
        }

        /// <summary>
        /// Compares this instance with another tuple, ordering primarily by the stored <see cref="Value"/> and secondarily by
        /// <see cref="Count"/> to enable deterministic sorting of frequency data.
        /// </summary>
        /// <param name="other">The other tuple participating in the comparison. A null reference is considered less than this instance.</param>
        /// <returns>A signed integer describing the relative order: negative when this tuple is less than <paramref name="other"/>, zero when equal, and positive when greater.</returns>
        public int CompareTo(ValueCountTuple other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            var result = Value.CompareTo(other.Value);
            if (result != 0)
                return result;
            return Count.CompareTo(other.Count);
        }

        /// <summary>
        /// Computes a hash code by combining the hash codes for the numeric value and occurrence count, providing deterministic
        /// hashing suitable for dictionary or set usage.
        /// </summary>
        /// <returns>An integer hash code derived from <see cref="Value"/> and <see cref="Count"/>.</returns>
        public override int GetHashCode()
        {

            unchecked
            {
                var hashCode = 1519435568;
                hashCode = (hashCode * 397) ^ Value.GetHashCode();
                hashCode = (hashCode * 397) ^ Count.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Determines whether the supplied object represents the same value-count pairing as this instance, ignoring reference
        /// identity when the stored data matches.
        /// </summary>
        /// <param name="obj">The object to compare with this tuple. Non-<see cref="ValueCountTuple"/> instances are treated as unequal.</param>
        /// <returns><see langword="true"/> when <paramref name="obj"/> stores equal value and count data; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(null, obj)) return false;

            return Equals(obj as ValueCountTuple);
        }

        /// <summary>
        /// Determines whether another tuple contains the same numeric value and occurrence count as this instance, enabling
        /// equality comparisons independent of reference identity.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance. A null reference is treated as unequal.</param>
        /// <returns><see langword="true"/> when both the value and count match; otherwise, <see langword="false"/>.</returns>
        public bool Equals(ValueCountTuple other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;

            return Value == other.Value && Count == other.Count;
        }

        /// <summary>
        /// Creates a textual representation of the tuple combining the numeric value with the count enclosed in brackets,
        /// matching the "value [count]" pattern for diagnostic display.
        /// </summary>
        /// <returns>A culture-sensitive string concatenating <see cref="Value"/> and <see cref="Count"/> in the format "value [count]".</returns>
        public override string ToString()
        {
            return Value.ToString() + " [" + Count.ToString() + "]";
        }
    }
}

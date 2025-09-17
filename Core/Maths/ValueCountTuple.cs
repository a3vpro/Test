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
    /// Replacing ValueTuple by class,
    /// intellisence didn't work in sqlproj with C# 7.0
    /// </summary>
    public class ValueCountTuple : IComparable, IComparable<ValueCountTuple>, IEquatable<ValueCountTuple>
    {
        /// <summary>
        /// Gets or sets the value associated with this instance.
        /// This property represents the main value of the tuple.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the count associated with this instance.
        /// This property represents the number of occurrences of the value.
        /// </summary>
        public int Count { get; set; }

        /// <summary> The ValueCountTuple function is a tuple that contains the value and count of each unique value in an array.</summary>
        /// <param name="value"> The value of the tuple.</param>
        /// <param name="count"> The number of times the value occurs.</param>
        /// <returns> The value and count of the tuple.</returns>
        public ValueCountTuple(double value, int count)
        {
            Value = value;
            Count = count;
        }

        /// <summary> The CompareTo function compares two objects of the same type and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
        /// <param name="obj"> What is this parameter used for?</param>
        /// <returns> The difference between the count of this object and the count of obj.</returns>
        public int CompareTo(object obj)
        {
            if (ReferenceEquals(this, obj)) return 0;
            if (ReferenceEquals(null, obj)) return 1;

            return CompareTo(obj as ValueCountTuple);
        }

        /// <summary> The CompareTo function is used to compare two objects of the same type and return an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <param name="other"> The other valuecounttuple to compare this one with.</param>
        /// <returns> An integer that indicates the relative order of the objects being compared. the return value has these meanings:
        ///less than zero - this object is less than the other parameter.
        ///zero - this object is equal to other. 
        ///greater than zero - this object is greater than other.</returns>
        public int CompareTo(ValueCountTuple other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            var result = Value.CompareTo(other.Value);
            if (result != 0)
                return result;
            return Count.CompareTo(other.Count);
        }

        /// <summary> The GetHashCode function is used to generate a unique hash code for each instance of the class.
        /// This function is called when an object needs to be stored in a HashSet or Dictionary.</summary>
        /// <returns> The hash code for the current instance.</returns>
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

        /// <summary> The Equals function is used to compare two objects of the same type.
        /// The function returns true if the objects are equal, and false otherwise.</summary>
        /// <param name="obj"> The object to compare with the current object.</param>
        /// <returns> True if the objects are of the same type and have equal values.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(null, obj)) return false;

            return Equals(obj as ValueCountTuple);
        }

        /// <summary> The Equals function is used to compare two objects of the same type.
        /// The function returns true if both objects are equal, and false otherwise.</summary>
        /// <param name="other"> The other valuecounttuple to compare</param>
        /// <returns> True if the value and count are equal.</returns>
        public bool Equals(ValueCountTuple other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;

            return Value == other.Value && Count == other.Count;
        }

        /// <summary> The ToString function returns a string representation of the object.</summary>
        /// <returns> A string that represents the current object.</returns>
        public override string ToString()
        {
            return Value.ToString() + " [" + Count.ToString() + "]";
        }
    }
}

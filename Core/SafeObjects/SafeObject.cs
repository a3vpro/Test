//----------------------------------------------------------------------------
// Author : Álvaro Ibáñez del Pino
// NickName : aibanez
// Created : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 18-11-2023
// Description : v1.7.1
//
// Copyright : (C) 2023 by Sothis/Nunsys. All rights reserved.
//----------------------------------------------------------------------------
using System;
using VisionNet.Core.Comparisons;
using VisionNet.Core.Types;
using VisionNet.Core.Exceptions;

namespace VisionNet.Core.SafeObjects
{
    /// <summary>
    /// Wraps an underlying <see cref="object"/> with strong, explicit type intent (<see cref="DataType"/>),
    /// safe conversion semantics, and thread-safe access. The wrapper helps avoid null dereferences and
    /// invalid casts by centralizing conversion, validation, and equality checks.
    /// </summary>
    /// <remarks>
    /// <para><b>Thread-safety:</b> Access to the underlying value is synchronized via an internal lock, so
    /// concurrent reads/writes from multiple threads are safe at the instance level.</para>
    /// <para><b>Type intent:</b> <see cref="DataType"/> indicates the intended <see cref="TypeCode"/> for the value.
    /// All <see cref="TrySetValue(object)"/> operations convert to that type (honoring <see cref="Preferences"/>).
    /// </para>
    /// <para><b>Defaults:</b> <see cref="DefaultValue"/> is used to initialize and <see cref="Clear"/> the value, and
    /// as the fallback for conversions in some operations.</para>
    /// </remarks>
    public class SafeObject : ISafeObject<TypeCode>, IEquatable<object>, IEquatable<SafeObject>
    {
        protected object _lockObject = new object();
        protected object _value;
        /// <summary>
        /// Gets the default value associated with this instance. This value is used to initialize the object,
        /// to reset it via <see cref="Clear"/>, and as a fallback when conversions request a default.
        /// </summary>
        /// <value>
        /// An <see cref="object"/> that is compatible with <see cref="DataType"/>. May be <c>null</c> for
        /// reference-like types or when <see cref="DataType"/> is <see cref="TypeCode.Empty"/>.
        /// </value>
        public object DefaultValue { get; protected set; }

        /// <summary>
        /// Gets the intended <see cref="TypeCode"/> target for conversions and storage.
        /// </summary>
        /// <value>
        /// A <see cref="TypeCode"/> describing the type this <see cref="SafeObject"/> attempts to maintain.
        /// The parameterless constructor sets this to <see cref="TypeCode.Empty"/>.
        /// </value>
        public TypeCode DataType { get; protected set; }

        /// <summary>
        /// Gets or sets the conversion preferences that influence how <see cref="TrySetValue(object)"/> and
        /// <see cref="TryGetValue{T}(out T)"/> attempt to coerce types (e.g., culture-aware strings, clamping, etc.).
        /// </summary>
        /// <value>
        /// A combination of <see cref="TypeConversionPreferences"/> flags. Defaults to
        /// <see cref="TypeConversionPreferences.None"/> unless provided in the constructor.
        /// </value>
        public TypeConversionPreferences Preferences { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="SafeObject"/> with no type intent and a <c>null</c> value.
        /// </summary>
        /// <remarks>
        /// Sets <see cref="DataType"/> to <see cref="TypeCode.Empty"/>, <see cref="DefaultValue"/> to <c>null</c>,
        /// and the underlying value to <c>null</c>. No conversion preferences are applied.
        /// </remarks>
        /// <exception cref="System.Exception">
        /// This constructor does not throw by design.
        /// </exception>
        public SafeObject()
        {
            DataType = TypeCode.Empty;
            DefaultValue = null;
            _value = null;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SafeObject"/> with an explicit <see cref="TypeCode"/>,
        /// a default value (optional), and conversion <see cref="Preferences"/>.
        /// </summary>
        /// <param name="dataType">The target <see cref="TypeCode"/> this instance should maintain.</param>
        /// <param name="defaultValue">
        /// Optional default. If <c>null</c>, it is set to the type's default via <see cref="TypeCodeExtensions.DefaultValue(TypeCode)"/>.
        /// If provided, it must be convertible to <paramref name="dataType"/>.
        /// </param>
        /// <param name="preferences">Conversion behavior flags used by conversions on this object.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="defaultValue"/> is provided but cannot be converted to <paramref name="dataType"/>.
        /// </exception>
        public SafeObject(TypeCode dataType, object defaultValue = null, TypeConversionPreferences preferences = TypeConversionPreferences.None)
        {
            if (defaultValue == null)
                defaultValue = dataType.DefaultValue();
            else if (!defaultValue.TryChangeType(dataType, out var _))
                throw new ArgumentException("The specified default value is not valid for the datatype");

            DataType = dataType;
            Preferences = preferences;
            DefaultValue = defaultValue;
            _value = defaultValue;
        }

        /// <summary>
        /// Retrieves the current underlying value with thread-safe access.
        /// </summary>
        /// <returns>
        /// The stored value as an <see cref="object"/>. It may be <c>null</c> depending on the current state and
        /// <see cref="DataType"/>.
        /// </returns>
        /// <remarks>
        /// Uses internal locking to ensure a consistent read under concurrent access.
        /// </remarks>
        public virtual object GetValue()
        {
            // SAFETY: synchronize read to avoid races with concurrent writers.
            lock (_lockObject)
                return _value;
        }

        /// <summary>
        /// Attempts to obtain the value converted to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The desired destination type.</typeparam>
        /// <param name="value">On success, receives the converted value; otherwise set to <c>default(T)</c>.</param>
        /// <returns>
        /// <c>true</c> if the value was already assignable to <typeparamref name="T"/> or could be converted to it;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The operation is thread-safe and will not throw on conversion failures; it logs and returns <c>false</c>.
        /// </remarks>
        /// <exception cref="System.Exception">
        /// Unexpected exceptions during conversion are caught and logged; the method returns <c>false</c>.
        /// </exception>
        public virtual bool TryGetValue<T>(out T value)
        {
            bool canConvert = false;
            value = default(T);
            try
            {
                lock (_lockObject)
                {
                    canConvert = SafeObjectHelper.IsAssignableFrom<T>(_value);
                    if (canConvert)
                        value = (T)_value;
                    else
                    {
                        canConvert = _value.TryChangeType(typeof(T), out var objValue, default(T));
                        if (canConvert)
                            value = (T)objValue;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(TryGetValue));
            }
            return canConvert;
        }

        /// <summary>
        /// Attempts to set the underlying value after converting it to <see cref="DataType"/>, honoring <see cref="Preferences"/>.
        /// </summary>
        /// <param name="value">The candidate value to store. May be any <see cref="object"/> convertible to <see cref="DataType"/>.</param>
        /// <returns>
        /// <c>true</c> if the value could be converted and was stored; otherwise <c>false</c>. When <c>false</c>,
        /// the previous value remains unchanged.
        /// </returns>
        /// <remarks>
        /// The operation is thread-safe and fail-safe: conversion problems are caught, logged, and reported via the return value.
        /// </remarks>
        /// <exception cref="System.Exception">
        /// Unexpected exceptions are caught and logged; the method returns <c>false</c>.
        /// </exception>
        public virtual bool TrySetValue(object value)
        {
            bool canConvert = false;
            try
            {
                lock (_lockObject)
                {
                    canConvert = value.TryChangeType(DataType, out var tmpValue, DefaultValue, Preferences);

                    if (canConvert)
                        _value = tmpValue;
                }
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(TrySetValue));
            }
            return canConvert;
        }

        /// <summary>
        /// Gets or sets the current value, using the safe conversion pipeline.
        /// </summary>
        /// <value>
        /// The current stored value. Getting is thread-safe and non-throwing. Setting attempts conversion to
        /// <see cref="DataType"/>; failures do not throw and leave the previous value intact.
        /// </value>
        /// <remarks>
        /// The setter delegates to <see cref="TrySetValue(object)"/>; callers needing to detect failure should call it directly.
        /// </remarks>
        public object Value
        {
            get => GetValue();
            set => TrySetValue(value);
        }

        /// <summary>
        /// Resets the underlying value to <see cref="DefaultValue"/>.
        /// </summary>
        /// <remarks>
        /// Thread-safe. No conversion is performed; the default is directly assigned.
        /// </remarks>
        /// <exception cref="System.Exception">
        /// This method does not throw by design.
        /// </exception>
        public virtual void Clear()
        {
            lock (_lockObject)
                _value = DefaultValue;
        }

        /// <summary>
        /// Validates whether a provided <paramref name="value"/> can be converted to <see cref="DataType"/>
        /// under the current <see cref="Preferences"/>.
        /// </summary>
        /// <param name="value">The value to test for convertibility.</param>
        /// <returns><c>true</c> if the value can be converted; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method does not mutate the internal state. It is thread-safe and does not throw for conversion failures.
        /// </remarks>
        public virtual bool IsValidValue(object value)
        {
            // BUGFIX: validate the provided value, not the current stored value.
            lock (_lockObject)
                return value.TryChangeType(DataType, out var tmpValue, DefaultValue, Preferences);
        }

        /// <summary>
        /// Converts the current value to <see cref="bool"/>, or returns the provided default when conversion fails.
        /// </summary>
        /// <param name="defaultValue">Fallback result when conversion is not possible. Defaults to <c>false</c>.</param>
        /// <returns>The converted boolean, or <paramref name="defaultValue"/> on failure.</returns>
        /// <remarks>
        /// Honors <see cref="Preferences"/> for string/number boolean conversions as configured by the implementation.
        /// </remarks>
        public virtual bool ToBool(bool defaultValue = false)
        {
            var success = TryGetValue<bool>(out var value);
            return success ? value : defaultValue;
        }

        /// <summary>
        /// Determines whether the current value can be interpreted as boolean <c>true</c>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if conversion succeeds and the value is <c>true</c>; otherwise, <c>false</c>.
        /// When conversion fails, this method returns <c>false</c>.
        /// </returns>
        public virtual bool IsTrue()
        {
            return ToBool(false);
        }

        /// <summary>
        /// Determines whether the current value can be interpreted as boolean <c>false</c>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if conversion succeeds and the value is <c>false</c>; otherwise, <c>false</c>.
        /// When conversion fails, this method returns <c>false</c>.
        /// </returns>
        public virtual bool IsFalse()
        {
            return !ToBool(true);
        }

        /// <summary>
        /// Converts the current value to <see cref="double"/>, or returns the provided default when conversion fails.
        /// </summary>
        /// <param name="defaultValue">Fallback result when conversion is not possible. Defaults to <c>0</c>.</param>
        /// <returns>The converted <see cref="double"/>, or <paramref name="defaultValue"/> on failure.</returns>
        public double ToFloat(double defaultValue = 0)
        {
            var success = TryGetValue<double>(out var value);
            return success ? value : defaultValue;
        }

        /// <summary>
        /// Converts the current value to <see cref="long"/>, or returns the provided default when conversion fails.
        /// </summary>
        /// <param name="defaultValue">Fallback result when conversion is not possible. Defaults to <c>0</c>.</param>
        /// <returns>The converted <see cref="long"/>, or <paramref name="defaultValue"/> on failure.</returns>
        public long ToInt(long defaultValue = 0)
        {
            var success = TryGetValue<long>(out var value);
            return success ? value : defaultValue;
        }

        /// <summary>
        /// Converts the current value to <see cref="string"/>, or returns the provided default when conversion fails.
        /// </summary>
        /// <param name="defaultValue">Fallback string when conversion is not possible. Defaults to <c>""</c>.</param>
        /// <returns>The converted string, or <paramref name="defaultValue"/> on failure.</returns>
        public string ToString(string defaultValue = "")
        {
            var success = TryGetValue<string>(out var value);
            // BUGFIX: if conversion fails, return the provided defaultValue instead of always empty string.
            return success ? value : defaultValue;
        }

        /// <summary>
        /// Determines value-based equality with another <see cref="SafeObject"/>.
        /// </summary>
        /// <param name="other">The other instance to compare against. May be <c>null</c>.</param>
        /// <returns>
        /// <c>true</c> if both wrappers hold values that are considered equal by <c>SafeAreEqualTo</c>;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Equality is determined by comparing the underlying values, not wrapper identity.
        /// </remarks>
        public bool Equals(SafeObject other)
        {
            return ComparisonExtension.Equals(_value, other?._value);
        }

        /// <summary>
        /// Determines equality against any <see cref="object"/> using value semantics when possible.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is the same reference as this instance, another <see cref="SafeObject"/>
        /// with an equal underlying value, or any object equal to the underlying value; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return ComparisonExtension.Equals(_value, obj);
        }

        /// <summary>
        /// Returns a hash code consistent with value-based equality of the underlying stored value.
        /// </summary>
        /// <returns>
        /// The hash code of the underlying value if non-<c>null</c>; otherwise <c>0</c>.
        /// </returns>
        /// <remarks>
        /// Uses locking to ensure a consistent snapshot under concurrent access.
        /// </remarks>
        public override int GetHashCode()
        {
            // POSTCONDITION: hash consistent with Equals based on the wrapped value.
            lock (_lockObject)
                return _value?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Compares two <see cref="SafeObject"/> instances for value-based equality.
        /// </summary>
        /// <param name="left">The first instance to compare. May be <c>null</c>.</param>
        /// <param name="right">The second instance to compare. May be <c>null</c>.</param>
        /// <returns>
        /// <c>true</c> if both are <c>null</c> or their underlying values are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(SafeObject left, SafeObject right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines inequality between two <see cref="SafeObject"/> instances.
        /// </summary>
        /// <param name="left">The first instance to compare. May be <c>null</c>.</param>
        /// <param name="right">The second instance to compare. May be <c>null</c>.</param>
        /// <returns>
        /// <c>true</c> if value-based equality (see <see cref="operator ==(SafeObject, SafeObject)"/>) is <c>false</c>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(SafeObject left, SafeObject right)
        {
            return !Equals(left, right);
        }
    }
}
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VisionNet.Core.SafeObjects
{
    /// <summary>
    /// A collection of safe objects, providing thread-safe access and manipulation methods.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used to identify each safe object.</typeparam>
    /// <typeparam name="TValue">The type of the safe object.</typeparam>
    /// <typeparam name="TType">The type of the value inside the safe object.</typeparam>
    public class SafeObjectCollection<TKey, TValue, TType> : ISafeObjectCollection<TKey, TValue, TType>
        where TValue : ISafeObject<TType>
    {
        /// <summary>
        /// A dictionary to store the safe objects by their keys.
        /// </summary>
        protected ConcurrentDictionary<TKey, TValue> _values = new ConcurrentDictionary<TKey, TValue>();

        /// <summary>
        /// Converts the safe object corresponding to the given key to a boolean value.
        /// </summary>
        /// <param name="key">The key of the safe object.</param>
        /// <param name="defaultValue">The default boolean value to return if the conversion fails.</param>
        /// <returns>The boolean value of the safe object.</returns>
        public bool ToBool(TKey key, bool defaultValue = false)
        {
            return _values.TryGetValue(key, out var safeObject)
                && safeObject.ToBool(defaultValue);
        }

        /// <summary>
        /// Determines whether the safe object corresponding to the given key is true.
        /// </summary>
        /// <param name="key">The key of the safe object.</param>
        /// <returns>True if the safe object is true, otherwise false.</returns>
        public bool IsTrue(TKey key)
        {
            return ToBool(key, false);
        }

        /// <summary>
        /// Determines whether the safe object corresponding to the given key is false.
        /// </summary>
        /// <param name="key">The key of the safe object.</param>
        /// <returns>True if the safe object is false, otherwise false.</returns>
        public bool IsFalse(TKey key)
        {
            return !ToBool(key, true);
        }

        /// <summary>
        /// Validates whether the given value is valid for the safe object identified by the given key.
        /// </summary>
        /// <param name="key">The key of the safe object.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value is valid for the safe object, otherwise false.</returns>
        public bool IsValidValue(TKey key, object value)
        {
            return _values.TryGetValue(key, out var safeObject)
                && safeObject.IsValidValue(value);
        }

        /// <summary>
        /// Converts the safe object corresponding to the given key to a floating-point value.
        /// </summary>
        /// <param name="key">The key of the safe object.</param>
        /// <param name="defaultValue">The default floating-point value to return if the conversion fails.</param>
        /// <returns>The floating-point value of the safe object.</returns>
        public double ToFloat(TKey key, double defaultValue = 0D)
        {
            return _values.TryGetValue(key, out var safeObject)
                ? safeObject.ToFloat(defaultValue)
                : defaultValue;
        }

        /// <summary>
        /// Converts the safe object corresponding to the given key to an integer value.
        /// </summary>
        /// <param name="key">The key of the safe object.</param>
        /// <param name="defaultValue">The default integer value to return if the conversion fails.</param>
        /// <returns>The integer value of the safe object.</returns>
        public long ToInt(TKey key, long defaultValue = 0)
        {
            return _values.TryGetValue(key, out var safeObject)
                ? safeObject.ToInt(defaultValue)
                : defaultValue;
        }

        /// <summary>
        /// Converts the safe object corresponding to the given key to a string value.
        /// </summary>
        /// <param name="key">The key of the safe object.</param>
        /// <param name="defaultValue">The default string value to return if the conversion fails.</param>
        /// <returns>The string value of the safe object.</returns>
        public string ToString(TKey key, string defaultValue = "")
        {
            return _values.TryGetValue(key, out var safeObject)
                ? safeObject.ToString(defaultValue)
                : defaultValue;
        }

        /// <summary>
        /// Inserts a new safe object into the collection with the specified key.
        /// </summary>
        /// <param name="id">The key to associate with the safe object.</param>
        /// <param name="entity">The safe object to insert.</param>
        public void Insert(TKey id, TValue entity)
        {
            _values.TryAdd(id, entity);
        }

        /// <summary>
        /// Removes the safe object identified by the given key from the collection.
        /// </summary>
        /// <param name="id">The key of the safe object to remove.</param>
        public void Delete(TKey id)
        {
            _values.TryRemove(id, out var entity);
        }

        /// <summary>
        /// Updates the safe object identified by the given key using the provided update action.
        /// </summary>
        /// <param name="id">The key of the safe object to update.</param>
        /// <param name="updateAction">The action to perform on the safe object.</param>
        /// <returns>The updated safe object.</returns>
        public TValue Update(TKey id, Func<TValue, TValue> updateAction)
        {
            TValue result = default(TValue);
            var success = _values.TryGetValue(id, out result);
            if (success)
            {
                result = updateAction(result);
                _values.AddOrUpdate(id, result, (i, e) => e);
            }
            return result;
        }

        /// <summary>
        /// Retrieves the safe object corresponding to the given key.
        /// </summary>
        /// <param name="id">The key of the safe object to retrieve.</param>
        /// <returns>The safe object identified by the key, or null if not found.</returns>
        public TValue Get(TKey id)
        {
            _values.TryGetValue(id, out var entity);
            return entity;
        }

        /// <summary>
        /// Retrieves all the safe objects in the collection as a list.
        /// </summary>
        /// <returns>A list of all the safe objects in the collection.</returns>
        public IList<TValue> GetAll()
        {
            return _values.Values.ToList();
        }

        /// <summary>
        /// Checks if a safe object with the specified key exists in the collection.
        /// </summary>
        /// <param name="id">The key to check for existence.</param>
        /// <returns>True if a safe object with the given key exists, otherwise false.</returns>
        public bool Exists(TKey id)
        {
            return _values.ContainsKey(id);
        }

        /// <summary>
        /// Gets the count of safe objects in the collection.
        /// </summary>
        /// <returns>The number of safe objects in the collection.</returns>
        public int Count()
        {
            return _values.Count;
        }

        /// <summary>
        /// Tries to set a value for the safe object identified by the given key.
        /// </summary>
        /// <param name="key">The key of the safe object to set the value for.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value was successfully set, otherwise false.</returns>
        public virtual bool TrySetValue(TKey key, object value)
        {
            bool success = _values.ContainsKey(key);
            if (success)
            {
                success = _values.TryGetValue(key, out var entity);
                if (success)
                    success = entity.TrySetValue(value);
            }
            return success;
        }

        /// <summary>
        /// Tries to retrieve a value from the safe object identified by the given key.
        /// </summary>
        /// <param name="key">The key of the safe object to retrieve the value from.</param>
        /// <param name="value">The value retrieved from the safe object.</param>
        /// <returns>True if the value was successfully retrieved, otherwise false.</returns>
        public virtual bool TryGetValue(TKey key, out object value)
        {
            bool success = _values.ContainsKey(key);
            value = null;
            if (success)
            {
                success = _values.TryGetValue(key, out var entity);
                if (success)
                    success = entity.TryGetValue(out value);
            }
            return success;
        }

        /// <summary>
        /// Tries to retrieve a value of type <typeparamref name="TType"/> from the safe object identified by the given key.
        /// </summary>
        /// <param name="key">The key of the safe object to retrieve the value from.</param>
        /// <param name="value">The value of type <typeparamref name="TType"/> retrieved from the safe object, if found. The default value of <typeparamref name="TType"/> if the retrieval fails.</param>
        /// <returns>True if the value of type <typeparamref name="TType"/> was successfully retrieved, otherwise false.</returns>
        public virtual bool TryGetValue<T>(TKey key, out T value)
        {
            bool success = _values.ContainsKey(key);
            value = default(T);
            if (success)
            {
                success = _values.TryGetValue(key, out var entity);
                if (success)
                    success = entity.TryGetValue<T>(out value);
            }
            return success;
        }

        /// <summary>
        /// Gets or sets the value of the safe object identified by the given key.
        /// </summary>
        /// <param name="key">The key of the safe object.</param>
        /// <returns>The value of the safe object.</returns>
        public object this[TKey key]
        {
            get
            {
                TryGetValue(key, out var value);
                return value;
            }
            set
            {
                TrySetValue(key, value);
            }
        }

        /// <summary>
        /// Clears all safe objects from the collection.
        /// </summary>
        public void Clear()
        {
            _values.Clear();
        }
    }
}

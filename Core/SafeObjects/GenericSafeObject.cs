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
using VisionNet.Core.Exceptions;

namespace VisionNet.Core.SafeObjects
{
    public class GenericSafeObject : ISafeObject<Type>
    {
        protected object _lockObject = new object();
        protected object _value;

        public Type DataType { get; protected set; }

        
        /// <summary> The GenericSafeObject function is a generic class that allows for the storage of any type of object.
        /// The GenericSafeObject function also provides methods to retrieve and set the value stored in it.</summary>
        /// <returns> The value of the property.</returns>
        public GenericSafeObject()
        {
            DataType = typeof(object);
            _value = null;
        }

        
        /// <summary> The GenericSafeObject function is a generic class that allows you to safely store and retrieve data of any type.
        /// It also provides the ability to set default values for each type, as well as a way to check if the value has been changed from its default.</summary>
        /// <param name="Type dataType"> The type of the data</param>
        /// <param name="object defaultValue"> The default value to use if the object is null.
        /// </param>
        /// <returns> The value of the genericsafeobject.</returns>
        public GenericSafeObject(Type dataType, object defaultValue)
        {
            DataType = dataType;
            _value = defaultValue;
        }

        
        /// <summary> The GetValue function returns the value of the object.</summary>
        /// <returns> The value of the variable.</returns>
        public virtual object GetValue()
        {
            lock (_lockObject)
                return _value;
        }

        
        /// <summary> The GetValue function returns the value of the object if it is assignable to type T.
        /// If not, then it returns default(T).</summary>
        /// <returns> The value of the property, cast to type t.</returns>
        public virtual T GetValue<T>()
        {
            lock (_lockObject)
            {
                var result = SafeObjectHelper.IsAssignableFrom<T>(_value);
                if (result)
                    return (T)_value;
            }
            return default(T);
        }

        
        /// <summary> The TryGetValue function attempts to retrieve the value of a given type from the SafeObject.
        /// If it is successful, then it returns true and sets the out parameter to that value. Otherwise, 
        /// if there is no such value or if there was an error retrieving it, then TryGetValue returns false.</summary>
        /// <param name="out T value"> The value to be returned</param>
        /// <returns> True if the value is of type t, otherwise false.</returns>
        public virtual bool TryGetValue<T>(out T value)
        {
            bool result = false;
            value = default(T);
            try
            {
                lock (_lockObject)
                {
                    result = SafeObjectHelper.IsAssignableFrom<T>(_value);
                    if (result)
                        value = (T)_value;
                }
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(TryGetValue));
            }
            return result;
        }

        
        /// <summary> The TrySetValue function attempts to set the value of a variable.
        /// If the value is valid, it will be set and true will be returned.
        /// Otherwise, false will be returned.</summary>
        /// <param name="object value"> What is this parameter used for?</param>
        /// <returns> True if the value is valid and set, otherwise false.</returns>
        public virtual bool TrySetValue(object value)
        {
            bool result = false;
            try
            {
                result = IsValidValue(value);
                if (result)
                    lock(_lockObject)
                        _value = value;
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(TrySetValue));
            }
            return result;
        }

        
        /// <summary> The IsValidValue function determines whether the value is valid for this property.</summary>
        /// <param name="object value"> The value to be validated.</param>
        /// <returns> True if the value is of the correct type, false otherwise.</returns>
        public virtual bool IsValidValue(object value)
        {
            return SafeObjectHelper.IsAssignableFrom(DataType, value);
        }
    }
}

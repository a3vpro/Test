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
using System.Collections.Generic;

namespace VisionNet.Core.Types
{
    public class TypeSwitchConverter<O>
    {
        Dictionary<Type, Func<object, O>> matches = new Dictionary<Type, Func<object, O>>();

        /// <summary> The Case function adds a new case to the switch statement.
        /// The first parameter is the type of object that will be matched, and
        /// the second parameter is a function that takes an object of type T and returns 
        /// an object of type O.</summary>
        /// <param name="Func&lt;object"> The type of the object.</param>
        /// <param name="func"> </param>
        /// <returns> A typeswitchconverter object.</returns>
        public TypeSwitchConverter<O> Case<T>(Func<object, O> func)
        {
            matches.Add(typeof(T), (x) => func(x));
            return this;
        }

        /// <summary> The Default function is used to set the default case for a TypeSwitch.
        /// If no other cases are matched, this function will be called.</summary>
        /// <param name="Func&lt;object"> The func&amp;lt;object, o&gt; is a delegate that represents the method that will handle the conversion.</param>
        /// <param name="func"> The function to exectute </param>
        /// <returns> An object of type o.</returns>
        public TypeSwitchConverter<O> Default(Func<object, O> func)
        {
            matches.Add(typeof(object), (x) => func(x));
            return this;
        }

        /// <summary> The Switch function is a pattern matching function that takes an object and returns the result of the first match found.
        /// The matches are defined in a dictionary, where each key is a type and each value is an action to perform on that type.</summary>
        /// <param name="x"> The object to be matched</param>
        /// <returns> The value of the function that matches the type of x.</returns>
        public O Switch(object x)
        {
            return matches[x.GetType()](x);
        }
    }
}
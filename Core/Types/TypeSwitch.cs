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
    public class TypeSwitch<O>
    {
        Dictionary<Type, Func<O>> matches = new Dictionary<Type, Func<O>>();

        /// <summary> The Case function adds a help to use a swith stantment fot type classes.
        /// The first parameter is the type of object that will be matched against.
        /// The second parameter is a function that returns an object of type O.</summary>
        /// <param name="func"> The function to be executed when the type matches. </param>
        /// <returns> A typeswitch object.</returns>
        public TypeSwitch<O> Case<T>(Func<O> func)
        {
            matches.Add(typeof(T), () => func());
            return this;
        }

        /// <summary> The Default function is used to set the default case for a TypeSwitch.
        /// This function should be called last in the chain of Case functions, and it will 
        /// return an instance of TypeSwitch&lt;O&gt; so that you can continue chaining Case functions.</summary>
        /// <param name="func"> This is the function that will be executed if no other case matches. </param>
        /// <returns> The value of the function passed in.</returns>
        public TypeSwitch<O> Default(Func<O> func)
        {
            matches.Add(typeof(object), () => func());
            return this;
        }

        /// <summary> The Switch function is a dictionary of functions that return an object.
        /// The key to the dictionary is a Type, and the value is a function that returns an object.
        /// This allows us to create objects based on their type.</summary>
        /// <param name="t"> The type of the object to be returned</param>
        /// <returns> The value of the function that is stored in matches[t]()</returns>
        public O Switch(Type t)
        {
            return matches[t]();
        }


        /// <summary> The Switch function is a generic function that takes in a type parameter T.
        /// The Switch function then returns the value of the matches dictionary at key typeof(T).
        /// This means that if you call Switch&lt;int&gt;(), it will return the value of matches[typeof(int)]().</summary>
        /// <returns> The value of the function that is associated with the type t.</returns>
        public O Switch<T>()
        {
            return matches[typeof(T)]();
        }
    }
}
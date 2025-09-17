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
using System.Linq;
using System.Reflection;

namespace VisionNet.Core.Attributes
{
    /// <summary>
    /// Provides extension methods for working with attributes in C#.
    /// </summary>
    public static class AttributeExtension
    {
        /// <summary>
        /// Checks if an object has a specific attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to check for.</typeparam>
        /// <param name="obj">The object to check for the attribute.</param>
        /// <returns>
        /// A boolean value: <c>true</c> if the object has the specified attribute, otherwise <c>false</c>.
        /// </returns>
        public static bool ClassHasAttribute<T>(this object obj)
            where T : Attribute
        {
            return obj.GetType()
                .ClassHasAttribute<T>();
        }

        /// <summary>
        /// Checks if a type has a specific attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to check for.</typeparam>
        /// <param name="type">The type to check for the attribute.</param>
        /// <returns>
        /// A boolean value: <c>true</c> if the type has the specified attribute, otherwise <c>false</c>.
        /// </returns>
        public static bool ClassHasAttribute<T>(this Type type)
            where T : Attribute
        {
            return type
                .GetCustomAttributes(true)
                .OfType<T>()
                .Any();
        }

        /// <summary>
        /// Checks if a method of a given object has a specific attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to check for.</typeparam>
        /// <param name="obj">The object containing the method to check.</param>
        /// <param name="methodName">The name of the method to check for the attribute.</param>
        /// <returns>
        /// A boolean value: <c>true</c> if the method has the specified attribute, otherwise <c>false</c>.
        /// </returns>
        public static bool MethodHasAttribute<T>(this object obj, string methodName)
            where T : Attribute
        {
            return obj.GetType()
                .MethodHasAttribute<T>(methodName);
        }

        /// <summary>
        /// Checks if a method of a given type has a specific attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to check for.</typeparam>
        /// <param name="type">The type containing the method to check.</param>
        /// <param name="methodName">The name of the method to check for the attribute.</param>
        /// <returns>
        /// A boolean value: <c>true</c> if the method has the specified attribute, otherwise <c>false</c>.
        /// </returns>
        public static bool MethodHasAttribute<T>(this Type type, string methodName)
            where T : Attribute
        {
            var methodInfo = type.GetMethod(methodName);
            return methodInfo != null && methodInfo
                .GetCustomAttributes(true)
                .OfType<T>()
                .Any();
        }

        /// <summary>
        /// Checks if a property of a given object has a specific attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to check for.</typeparam>
        /// <param name="obj">The object containing the property to check.</param>
        /// <param name="propertyName">The name of the property to check for the attribute.</param>
        /// <returns>
        /// A boolean value: <c>true</c> if the property has the specified attribute, otherwise <c>false</c>.
        /// </returns>
        public static bool PropertyHasAttribute<T>(this object obj, string propertyName)
            where T : Attribute
        {
            return obj.GetType()
                .PropertyHasAttribute<T>(propertyName);
        }

        /// <summary>
        /// Checks if a property of a given type has a specific attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to check for.</typeparam>
        /// <param name="type">The type containing the property to check.</param>
        /// <param name="propertyName">The name of the property to check for the attribute.</param>
        /// <returns>
        /// A boolean value: <c>true</c> if the property has the specified attribute, otherwise <c>false</c>.
        /// </returns>
        public static bool PropertyHasAttribute<T>(this Type type, string propertyName)
            where T : Attribute
        {
            var propertyInfo = type.GetProperty(propertyName);
            return propertyInfo != null && propertyInfo
                .GetCustomAttributes(true)
                .OfType<T>()
                .Any();
        }

        /// <summary>
        /// Returns a dictionary of methods and their associated attributes for a given object.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find on the methods.</typeparam>
        /// <param name="obj">The object whose methods are being checked for the attribute.</param>
        /// <returns>
        /// A dictionary where the keys are <see cref="MethodInfo"/> objects representing methods,
        /// and the values are lists of <typeparamref name="T"/> representing the attributes found on those methods.
        /// </returns>
        public static Dictionary<MethodInfo, List<T>> GetMethodsWithAttribute<T>(this object obj)
            where T : Attribute
        {
            return obj.GetType()
                .GetMethodsWithAttribute<T>();
        }

        /// <summary>
        /// Returns a dictionary of methods and their associated attributes for a given type.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find on the methods.</typeparam>
        /// <param name="type">The type whose methods are being checked for the attribute.</param>
        /// <returns>
        /// A dictionary where the keys are <see cref="MethodInfo"/> objects representing methods,
        /// and the values are lists of <typeparamref name="T"/> representing the attributes found on those methods.
        /// </returns>
        public static Dictionary<MethodInfo, List<T>> GetMethodsWithAttribute<T>(this Type type)
            where T : Attribute
        {
            return type.GetMethods()
                .Select(m =>
                    new
                    {
                        MethodInfo = m,
                        Attributes = m.GetCustomAttributes(true).OfType<T>().ToList()
                    })
                .Where(t => t.Attributes.Count > 0)
                .ToDictionary(t => t.MethodInfo, t => t.Attributes);
        }

        /// <summary>
        /// Returns a dictionary of the first method with the specified attribute for a given object.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find on the methods.</typeparam>
        /// <param name="obj">The object whose methods are being checked for the attribute.</param>
        /// <returns>
        /// A dictionary where the keys are <see cref="MethodInfo"/> objects representing methods,
        /// and the values are the first instance of <typeparamref name="T"/> found on those methods.
        /// </returns>
        public static Dictionary<MethodInfo, T> GetFirstMethodWithAttribute<T>(this object obj)
            where T : Attribute
        {
            return obj.GetType()
                .GetFirstMethodWithAttribute<T>();
        }

        /// <summary>
        /// Returns a dictionary of the first method with the specified attribute for a given type.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find on the methods.</typeparam>
        /// <param name="type">The type whose methods are being checked for the attribute.</param>
        /// <returns>
        /// A dictionary where the keys are <see cref="MethodInfo"/> objects representing methods,
        /// and the values are the first instance of <typeparamref name="T"/> found on those methods.
        /// </returns>
        public static Dictionary<MethodInfo, T> GetFirstMethodWithAttribute<T>(this Type type)
            where T : Attribute
        {
            return type.GetMethods()
                .Select(m =>
                    new
                    {
                        MethodInfo = m,
                        Attribute = m.GetCustomAttributes(true).OfType<T>().FirstOrDefault()
                    })
                .Where(t => t.Attribute != null)
                .ToDictionary(t => t.MethodInfo, t => t.Attribute);
        }

        /// <summary>
        /// Returns a dictionary of properties and their associated attributes for a given object.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find on the properties.</typeparam>
        /// <param name="obj">The object whose properties are being checked for the attribute.</param>
        /// <returns>
        /// A dictionary where the keys are <see cref="PropertyInfo"/> objects representing properties,
        /// and the values are lists of <typeparamref name="T"/> representing the attributes found on those properties.
        /// </returns>
        public static Dictionary<PropertyInfo, List<T>> GetPropertiesWithAttribute<T>(this object obj)
            where T : Attribute
        {
            return obj.GetType()
                .GetPropertiesWithAttribute<T>();
        }

        /// <summary>
        /// Returns a dictionary of properties and their associated attributes for a given type.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find on the properties.</typeparam>
        /// <param name="type">The type whose properties are being checked for the attribute.</param>
        /// <returns>
        /// A dictionary where the keys are <see cref="PropertyInfo"/> objects representing properties,
        /// and the values are lists of <typeparamref name="T"/> representing the attributes found on those properties.
        /// </returns>
        public static Dictionary<PropertyInfo, List<T>> GetPropertiesWithAttribute<T>(this Type type)
            where T : Attribute
        {
            return type.GetProperties()
                .Select(p =>
                    new
                    {
                        PropertyInfo = p,
                        Attributes = p.GetCustomAttributes(true).OfType<T>().ToList()
                    })
                .Where(t => t.Attributes.Count > 0)
                .ToDictionary(t => t.PropertyInfo, t => t.Attributes);
        }

        /// <summary>
        /// Returns the first property with the specified attribute for a given object.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find on the properties.</typeparam>
        /// <param name="obj">The object whose properties are being checked for the attribute.</param>
        /// <returns>
        /// A <see cref="PropertyInfo"/> representing the property, 
        /// and the first instance of <typeparamref name="T"/> found on that property.
        /// </returns>
        public static Dictionary<PropertyInfo, T> GetFirstPropertyWithAttribute<T>(this object obj)
            where T : Attribute
        {
            return obj.GetType()
                .GetFirstPropertyWithAttribute<T>();
        }

        /// <summary>
        /// Returns the first property with the specified attribute for a given type.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find on the properties.</typeparam>
        /// <param name="type">The type whose properties are being checked for the attribute.</param>
        /// <returns>
        /// A <see cref="PropertyInfo"/> representing the property, 
        /// and the first instance of <typeparamref name="T"/> found on that property.
        /// </returns>
        public static Dictionary<PropertyInfo, T> GetFirstPropertyWithAttribute<T>(this Type type)
            where T : Attribute
        {
            return type.GetProperties()
                .Select(p =>
                    new
                    {
                        PropertyInfo = p,
                        Attribute = p.GetCustomAttributes(true).OfType<T>().FirstOrDefault()
                    })
                .Where(t => t.Attribute != null)
                .ToDictionary(t => t.PropertyInfo, t => t.Attribute);
        }
    }
}


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
using System.Linq;
using System.Reflection;

namespace VisionNet.Core.Attributes
{
    /// <summary>
    /// Provides extension methods for working with <see cref="UriAddressAttribute"/> applied to objects and types.
    /// </summary>
    public static class UriAddressExtension
    {
        /// <summary>
        /// Retrieves the <see cref="UriAddressAttribute"/> applied to an object and returns its <see cref="Uri"/> property.
        /// </summary>
        /// <typeparam name="T">The type of the object to check for the attribute.</typeparam>
        /// <param name="obj">The object to check for the attribute.</param>
        /// <returns>
        /// The <see cref="Uri"/> associated with the <see cref="UriAddressAttribute"/> if found, otherwise <c>null</c>.
        /// </returns>
        public static Uri GetUriAddress<T>(this T obj)
        {
            var attribute = obj.GetType()
                                .GetCustomAttributes(true)
                                .OfType<UriAddressAttribute>()
                                .FirstOrDefault();

            if (attribute != null)
            {
                return attribute.Uri;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the <see cref="UriAddressAttribute.UriString"/> property of the <see cref="UriAddressAttribute"/> applied to an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to check for the attribute.</typeparam>
        /// <param name="obj">The object to check for the attribute.</param>
        /// <returns>
        /// The <see cref="UriAddressAttribute.UriString"/> value of the <see cref="UriAddressAttribute"/> if found, otherwise an empty string.
        /// </returns>
        public static string GetUriStringAddress<T>(this T obj)
        {
            var attribute = obj.GetType()
                                .GetCustomAttributes(true)
                                .OfType<UriAddressAttribute>()
                                .FirstOrDefault();

            if (attribute != null)
            {
                return attribute.UriString;
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves the <see cref="UriAddressAttribute"/> applied to a given type and returns its <see cref="Uri"/> property.
        /// </summary>
        /// <param name="type">The type to check for the attribute.</param>
        /// <returns>
        /// The <see cref="Uri"/> associated with the <see cref="UriAddressAttribute"/> if found, otherwise <c>null</c>.
        /// </returns>
        public static Uri GetUriAddress(this Type type)
        {
            var attribute = type.GetCustomAttributes(true)
                                .OfType<UriAddressAttribute>()
                                .FirstOrDefault();

            if (attribute != null)
            {
                return attribute.Uri;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the <see cref="UriAddressAttribute.UriString"/> property of the <see cref="UriAddressAttribute"/> applied to a given type.
        /// </summary>
        /// <param name="type">The type to check for the attribute.</param>
        /// <returns>
        /// The <see cref="UriAddressAttribute.UriString"/> value of the <see cref="UriAddressAttribute"/> if found, otherwise an empty string.
        /// </returns>
        public static string GetUriStringAddress(this Type type)
        {
            var attribute = type.GetCustomAttributes(true)
                               .OfType<UriAddressAttribute>()
                               .FirstOrDefault();

            if (attribute != null)
            {
                return attribute.UriString;
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves the type of an object associated with a specific <see cref="Uri"/> in a given assembly.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to check for.</param>
        /// <param name="assembly">The assembly to search for the type (default is the currently executing assembly).</param>
        /// <param name="baseType">The base type to filter the types (default is <see cref="object"/>).</param>
        /// <returns>
        /// The <see cref="Type"/> of the object that is associated with the specified <see cref="Uri"/>, or <c>null</c> if not found.
        /// </returns>
        public static Type GetTypeOfUri(Uri uri, Assembly assembly = null, Type baseType = null)
        {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            if (baseType == null)
                baseType = typeof(object);

            return assembly
                .GetTypes()
                .Where(t => baseType.IsAssignableFrom(t)
                    && GetUriAddress(t) == uri)
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the type of an object associated with a specific <see cref="UriAddressAttribute.UriString"/> in a given assembly.
        /// </summary>
        /// <param name="uriString">The <see cref="UriAddressAttribute.UriString"/> to check for.</param>
        /// <param name="assembly">The assembly to search for the type (default is the currently executing assembly).</param>
        /// <param name="baseType">The base type to filter the types (default is <see cref="object"/>).</param>
        /// <returns>
        /// The <see cref="Type"/> of the object that is associated with the specified <see cref="UriAddressAttribute.UriString"/>, or <c>null</c> if not found.
        /// </returns>
        public static Type GetTypeOfUri(string uriString, Assembly assembly = null, Type baseType = null)
        {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            if (baseType == null)
                baseType = typeof(object);

            return assembly
                .GetTypes()
                .Where(t => baseType.IsAssignableFrom(t)
                    && GetUriStringAddress(t) == uriString)
                .FirstOrDefault();
        }
    }
}

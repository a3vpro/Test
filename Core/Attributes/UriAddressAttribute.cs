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

namespace VisionNet.Core.Attributes
{
    /// <summary>
    /// Represents an attribute that is used to specify a URI address for a class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UriAddressAttribute : Attribute
    {
        /// <summary>
        /// Gets the URI object constructed from the <see cref="UriString"/>.
        /// </summary>
        public Uri Uri;

        /// <summary>
        /// Gets the string representation of the URI.
        /// </summary>
        public string UriString;

        /// <summary>
        /// Initializes a new instance of the <see cref="UriAddressAttribute"/> class.
        /// This constructor creates a <see cref="Uri"/> object from the provided <paramref name="uriString"/> and sets the <see cref="UriString"/> property.
        /// </summary>
        /// <param name="uriString">The URI string.</param>
        /// <param name="uriKind">The type of URI to create. The default value is <see cref="UriKind.Relative"/>.</param>
        public UriAddressAttribute(string uriString, UriKind uriKind = UriKind.Relative)
        {
            UriString = uriString;
            Uri = new Uri(UriString, uriKind);
        }
    }
}
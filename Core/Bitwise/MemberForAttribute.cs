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
using System.Diagnostics;

namespace VisionNet.Core.Bitwise
{
    /// <summary>
    /// Marker attribute to designate members which require special-casing other than <see cref="long"/>
    /// </summary>
    [Conditional("NEVER_RETAIN")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal sealed class MemberForAttribute : Attribute
    {
        public MemberForAttribute(Type type) { }
    }
}

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

namespace VisionNet.Core.Events
{
    /// <summary>
    /// A event args to query a condition
    /// </summary>
    public class ConditionEventArgs: EventArgs
    {
        /// <summary>
        /// The condition of the EventArgs
        /// </summary>
        public bool Condition { get; set; }

        /// <summary>
        /// Create a ConditionEventArgs
        /// </summary>
        public ConditionEventArgs()
        {
        }
    }
}

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
using System.Threading;

namespace VisionNet.Core.Tasks
{
    public class SequenceItem
    {
        public int DelayMs { get; set; } = 20;

        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        public Action<Dictionary<string, object>, CancellationTokenSource> Action { get; set; }
    }
}

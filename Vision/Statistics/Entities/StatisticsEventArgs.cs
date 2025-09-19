//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 23-06-2023
// Description      : v1.7.0
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.            
//-----------------------------------------------
using System;

namespace VisionNet.Vision.Core
{
    public class StatisticsEventArgs : EventArgs
    {
        public IResultStatisticsInfo Statistics { get; private set; }

        public StatisticsEventArgs(ResultStatistics statistics)
        {
            Statistics = statistics;
        }
    }
}
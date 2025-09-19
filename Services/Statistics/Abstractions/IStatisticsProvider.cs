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
using VisionNet.Core.Abstractions;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    public interface IStatisticsProvider : IService<ResultStatisticsOptions>
    {
        /// <summary>
        /// Get the statistics
        /// </summary>
        IResultStatisticsInfo Get { get; }

        /// <summary>
        /// Refresh the statistics
        /// </summary>
        /// <param name="dateTime"></param>
        void Refresh(DateTime dateTime = default);

        /// <summary>
        /// Reset the statistics
        /// </summary>
        void Reset();

        /// <summary>
        /// Save the statistics to restore automatically at start
        /// </summary>
        void Persist();
    }
}



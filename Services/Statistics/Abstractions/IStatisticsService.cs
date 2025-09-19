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
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    public interface IStatisticsService: IStatisticsProvider
    {
        void Update(ProductResult productResult, DateTime dateTime = default);

        /// <summary>
        /// Event raised on any statistics is updated
        /// </summary>
        event EventHandler<StatisticsEventArgs> StatisticsChanged;
    }
}



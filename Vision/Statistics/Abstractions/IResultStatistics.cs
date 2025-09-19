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
    public interface IResultStatistics : IStatistic, IResultStatisticsInfo
    {
        /// <summary>
        /// Update the statistics values
        /// </summary>
        /// <param name="inspectionResult"></param>
        /// <param name="dateTime"></param>
        void Update(ProductResult productResult, DateTime dateTime = default);

        bool IsValid(ProductResult productResult);

        void Repair(ProductResult productResult);
    }
}
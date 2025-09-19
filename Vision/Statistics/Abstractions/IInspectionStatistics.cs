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
    public interface IInspectionStatistics: IStatistic, IInspectionStatisticsInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inspectionResult"></param>
        /// <param name="dateTime"></param>
        void Update(InspectionResult inspectionResult, DateTime dateTime = default);

        event EventHandler<EventArgs> Updated;
    }
}
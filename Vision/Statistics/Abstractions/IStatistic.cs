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
    public interface IStatistic: IStatisticInfo
    {
        /// <summary>
        /// Reset the statisticas
        /// </summary>
        void Reset(DateTime dateTime = default);

        /// <summary>
        /// Refresh the statistics values
        /// </summary>
        /// <param name="dateTime">Moment of the statistics</param>
        void Refresh(DateTime dateTime = default);

        /// <summary>
        /// Update statistics with a new value
        /// </summary>
        /// <param name="matchCriteria">Value to update the statistics</param>
        /// <param name="dateTime">Moment of the statistics</param>
        void Update(bool matchCriteria, DateTime dateTime = default);
    }
}
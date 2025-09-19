//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 13-12-2023
//
// Last Modified By : aibanez
// Last Modified On : 13-12-2023
// Description      : v1.7.1
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.       
//----------------------------------------------------------------------------
using System.Collections.Generic;
using VisionNet.Core.Patterns;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Clase conversora entre alarmas de tipo IAlarmInfo a AlarmItem
    /// </summary>
    public class AlarmAdapter : IAdapter<IOutputParametersCollection2, ProductResult>
    {
        public ProductResult Convert(IOutputParametersCollection2 value)
        {
            var result = new ProductResult();
            Convert(value, ref result);
            return result;
        }

        public void Convert(IOutputParametersCollection2 value, ref ProductResult result)
        {
            //result.Info = new ProductInfoResult
            //{
            //    Source = Source,
            //    Enabled = Enabled,
            //    Result = Result,
            //    Success = Success,
            //    ProcessTime = ProcessTime,
            //    Error = Error,
            //    DateTime = DateTime,
            //    ByPass = ByPass,
            //    Forced = Forced,
            //    Index = Index,
            //};
            //result.Parameters = new ParametersResult
            //{
            //    Valid = Valid,
            //    Error = Error,
            //    Parameters = new List<ValueResult>
            //    {
            //        new ValueResult
            //        {
            //            Name = Name,
            //            Type = Type,
            //            Value = Value,
            //        }
            //    },
            //};
            //result.Images = new List<ImageResult>
            //{
            //    new ImageResult
            //    {
            //        Name = Name,
            //        FileName = FileName,
            //        Image = Image,
            //        Source = Source,
            //        AcquisitionMoment = AcquisitionMoment,
            //    }
            //};
            //result.Inspections = new List<InspectionResult>
            //{
            //    new InspectionResult
            //    {
            //        Name
            //        Enabled
            //        Result
            //        Success
            //        PrevResult
            //        ProcessTime
            //        Error
            //        Measurables = new List<ValueResult>
            //        {
            //            Name
            //            Type
            //            Value
            //        }
            //    }
            //};
        }
    }
}
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
using VisionNet.Vision.AI;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    public class SegmentationOutputParametersCollection : OutputParametersCollection
    {
        [Parameter(nameof(Instances),
            nameof(Instances),
            nameof(Instances),
            "All the instances found",
            ParameterDirection.Output,
            ParameterSource.Runtime,
            null,
            ParameterScope.Execution,
            BasicTypeCode.Object,
            true,
            TypeConversionPreferences.None,
            true,
            false)]
        public List<SingleInstSegmentationInferenceResult> Instances
        {
            get =>
                TryGetValue(nameof(Instances), out var value)
                    && value is List<SingleInstSegmentationInferenceResult> valueList
                    ? valueList
                    : null;
            set => TrySetValue(nameof(Instances), value);
        }
        public IParameter InstancesParameter =>
            Get(nameof(Instances));

        [Parameter(nameof(HasResult),
            nameof(HasResult),
            nameof(HasResult),
            "The inspection has detected any instance",
            ParameterDirection.Output,
            ParameterSource.Runtime,
            false,
            ParameterScope.Execution,
            BasicTypeCode.Boolean,
            false,
            TypeConversionPreferences.None,
            true,
            true)]
        public bool HasResult
        {
            get => this.ToBool(nameof(HasResult));
            set => TrySetValue(nameof(HasResult), value);
        }
        public IParameter HasResultParameter =>
            Get(nameof(HasResult));

        [Parameter(nameof(NumInstances),
            nameof(NumInstances),
            nameof(NumInstances),
            "Number of instances",
            ParameterDirection.Output,
            ParameterSource.Runtime,
            0,
            ParameterScope.Execution,
            BasicTypeCode.IntegerNumber,
            false,
            TypeConversionPreferences.None,
            true,
            true)]
        public long NumInstances
        {
            get => this.ToInt(nameof(NumInstances));
            set => TrySetValue(nameof(NumInstances), value);
        }
        public IParameter NumInstancesParameter =>
            Get(nameof(NumInstances));

        [Parameter(nameof(MinScore),
            nameof(MinScore),
            nameof(MinScore),
            "Mininum Score of the detected instances",
            ParameterDirection.Output,
            ParameterSource.Runtime,
            0.0,
            ParameterScope.Execution,
            BasicTypeCode.FloatingPointNumber,
            false,
            TypeConversionPreferences.None,
            true,
            true)]
        public double MinScore
        {
            get => this.ToFloat(nameof(MinScore));
            set => TrySetValue(nameof(MinScore), value);
        }
        public IParameter MinScoreParameter =>
            Get(nameof(MinScore));
    }
}

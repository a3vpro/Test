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
using System.Collections.Generic;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    public class OutputParametersCollection : ParametersCollection, IOutputParametersCollection, IMessageParametersCollection
    {
        public override ParameterDirection Direction => ParameterDirection.Output;

        [Parameter(nameof(Enabled), 
            nameof(Enabled), 
            nameof(Enabled), 
            nameof(Enabled), 
            ParameterDirection.Output, 
            ParameterSource.Runtime, 
            true, 
            ParameterScope.Execution, 
            BasicTypeCode.Boolean, 
            false, 
            TypeConversionPreferences.None,
            false,
            false)]
        public bool Enabled
        {
            get => this.ToBool(nameof(Enabled));
            set => TrySetValue(nameof(Enabled), value);
        }

        public IParameter EnabledParameter =>
            Get(nameof(Enabled));

        [Parameter(nameof(PrevResult), 
            nameof(PrevResult), 
            nameof(PrevResult), 
            "Previous result",
            ParameterDirection.Output, 
            ParameterSource.Runtime, 
            true, 
            ParameterScope.Execution, 
            BasicTypeCode.Boolean, 
            false, 
            TypeConversionPreferences.None,
            false,
            false)]
        public bool PrevResult
        {
            get => this.ToBool(nameof(PrevResult));
            set => TrySetValue(nameof(PrevResult), value);
        }

        public IParameter PrevResultParameter =>
            Get(nameof(PrevResult));

        [Parameter(nameof(Result), 
            nameof(Result), 
            nameof(Result), 
            nameof(Result), 
            ParameterDirection.Output, 
            ParameterSource.Runtime, 
            true, 
            ParameterScope.Execution, 
            BasicTypeCode.Boolean, 
            false, 
            TypeConversionPreferences.None,
            false,
            false)]
        public bool Result
        {
            get => this.ToBool(nameof(Result));
            set => TrySetValue(nameof(Result), value);
        }

        public IParameter ResultParameter =>
            Get(nameof(Result));

        [Parameter(nameof(Success), 
            nameof(Success), 
            nameof(Success), 
            "The execution has done without errors", 
            ParameterDirection.Output, 
            ParameterSource.Runtime, 
            true, 
            ParameterScope.Execution, 
            BasicTypeCode.Boolean, 
            false, 
            TypeConversionPreferences.None,
            false,
            false)]
        public bool Success
        {
            get => this.ToBool(nameof(Success));
            set => TrySetValue(nameof(Success), value);
        }

        public IParameter SuccessParameter =>
            Get(nameof(Success));

        [Parameter(nameof(Warning),
            nameof(Warning),
            nameof(Warning),
            nameof(Warning),
            ParameterDirection.Output,
            ParameterSource.Runtime,
            false,
            ParameterScope.Execution,
            BasicTypeCode.Boolean,
            false,
            TypeConversionPreferences.None,
            false,
            false)]
        public bool Warning
        {
            get => this.ToBool(nameof(Warning));
            set => TrySetValue(nameof(Warning), value);
        }

        public IParameter WarningParameter =>
            Get(nameof(Warning));

        public ResultStatus Status => ResultStatusHelper.Create(Result, Enabled, Success, PrevResult, Warning);

        [Parameter(nameof(ProcessTime), 
            nameof(ProcessTime), 
            nameof(ProcessTime), 
            "Total duration of the execution", 
            ParameterDirection.Output, 
            ParameterSource.Runtime, 
            0.0, 
            ParameterScope.Execution, 
            BasicTypeCode.FloatingPointNumber, 
            false, 
            TypeConversionPreferences.None,
            false,
            false)]
        public double ProcessTime
        {
            get => this.ToFloat(nameof(ProcessTime));
            set => TrySetValue(nameof(ProcessTime), value);
        }

        public IParameter ProcessTimeParameter =>
            Get(nameof(ProcessTime));


        [Parameter(nameof(Error), 
            nameof(Error), 
            nameof(Error), 
            "Text description of the error",
            ParameterDirection.Output, 
            ParameterSource.Runtime, 
            "", 
            ParameterScope.Execution, 
            BasicTypeCode.String, 
            false, 
            TypeConversionPreferences.None,
            false,
            false)]
        public string Error
        {
            get => this.ToString(nameof(Error));
            set => TrySetValue(nameof(Error), value);
        }

        public IParameter ErrorParameter =>
            Get(nameof(Error));
    }
}

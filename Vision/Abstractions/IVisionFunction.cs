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
using VisionNet.Core;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Events;
using VisionNet.Core.Patterns;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    public interface IVisionFunction: IEntity<string>, 
        INamed, 
        IDescriptible, 
        ISafeConfigureOptions<VisionFunctionOptions>, 
        IConfigureOptionsObservable<VisionFunctionOptions>,
        IInspectionFunctionExecutable, 
        IExecutionObservable<IParametersCollection>, 
        IExceptionObservable, 
        IPoolable,
        IRuntimeConfigurable<VisionFunctionOptions>
    {
        VisionFunctionType Type { get; }

        IInputParametersCollection InputParameters { get; }

        IOutputParametersCollection OutputParameters { get; }

        VisionFunctionOptions Options { get; }

        bool IncludeInResult { get; }

        IOutputParametersCollection CreateOutputParameters();

        IOutputParametersCollection NewEmptyOutputParameters(bool enabled, bool prevResult, bool result, bool success, bool warning, string error);

        InspectionResult NewEmptyInspectionResult(bool enabled, bool prevResult, bool result, bool success, bool warning, string error, List<NamedValue> step);

        IInputParametersCollection GetInputParameters(VisionMessage vm);

        void Initialize(params (string, object)[] parameters);

        void Execute(params (string, object)[] parameters);

        new InspectionResult Execute(IInputParametersCollection inputParameters, List<NamedValue> step);

        event EventHandler<EventArgs<ExecutionStatus>> ExecutionStatusChanged;
    }

    public interface IVisionFunction<TOptions, TInputParam, TOutputParam> : IVisionFunction
        where TOptions : VisionFunctionOptions, new()
        where TInputParam : IInputParametersCollection, new()
        where TOutputParam : IOutputParametersCollection, new()
    {
    }
}

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
using System.IO;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Events;
using VisionNet.Core.States;
using Conditions;
using VisionNet.Diagnostics;
using VisionNet.Core.Exceptions;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    public abstract class VisionFunction<TOptions, TInputParam, TOutputParam> : IVisionFunction<TOptions, TInputParam, TOutputParam>, IVisionFunction
        where TOptions : VisionFunctionOptions, new()
        where TInputParam : InputParametersCollection, new()
        where TOutputParam : OutputParametersCollection, new()
    {
        protected IRateMeter _durationMeter;

        protected readonly ExecutionStatusTransition _executionStatusTransition = new ExecutionStatusTransition();

        public string Index { get; protected set; } = string.Empty;

        public string Name { get; protected set; } = string.Empty;

        public string Description { get; protected set; } = string.Empty;

        private ExecutionStatus _status = ExecutionStatus.Initial;
        public ExecutionStatus Status
        {
            get => _status;
            protected set
            {
                _status = value;
                RaiseExecutionStatusChanged(this, new EventArgs<ExecutionStatus>(value));
            }
        }

        public abstract VisionFunctionType Type { get; }

        protected VisionFunctionOptions _options = new VisionFunctionOptions();

        /// <summary>
        /// Obtiene o establece las opciones de configuración de la función de visión.
        /// </summary>
        public VisionFunctionOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        /// <summary>
        /// Obtiene o establece las opciones personalizadas de configuración de la función de visión.
        /// </summary>
        public TOptions FunctionOptions
        {
            get { return _options as TOptions; }
            set { _options = value; }
        }

        protected InputParametersCollection _inputParameters;
        public IInputParametersCollection InputParameters => _inputParameters;
        public TInputParam InputParams => _inputParameters as TInputParam;

        protected OutputParametersCollection _outputParameters;
        public IOutputParametersCollection OutputParameters => _outputParameters;
        public TOutputParam OutputParams => _outputParameters as TOutputParam;

        protected bool _includeInResult;
        public bool IncludeInResult => _includeInResult;

        protected bool _includeInStats;
        public bool IncludeInStats => _includeInStats;

        protected bool _isSegmentationInspection;
        public bool IsSegmentationInspection => _isSegmentationInspection;

        public VisionFunction()
        {
            _inputParameters = new TInputParam();
            _outputParameters = new TOutputParam();
        }

        public void Configure(VisionFunctionOptions options)
        {
            if (_executionStatusTransition.IsValidTransition(Status, ExecutionStatus.Ready))
            {
                try
                {
                    options.Index.Requires(nameof(options.Index)).IsNotNullOrWhiteSpace();
                    options.Name.Requires(nameof(options.Name)).IsNotNullOrWhiteSpace();

                    // Falta comprobar que los parámetros de entrada y salida no tienen el mismo índice!!!!

                    Name = options.Name;
                    Index = options.Index;
                    Description = options.Description;

                    _includeInResult = options.IncludeInResult;
                    _includeInStats = options.IncludeInStats;
                    _isSegmentationInspection = options.IsSegmentationInspection;

                    _inputParameters.ParentName = Name;

                    _configureInputParameters((TOptions)options);

                    var outputParams = options.Parameters.Where(p => p.Direction == ParameterDirection.Output).ToArray();
                    _outputParameters.AddRange(outputParams);

                    _durationMeter = new SimpleRateMeter(Description, "VisionFunction", Index);

                    _configure(options);

                    _initialize();

                    Status = ExecutionStatus.Ready;
                    RaiseConfigured(this, new EventArgs<VisionFunctionOptions>(options));
                }
                catch (Exception ex)
                {
                    Status = ExecutionStatus.Error;
                    RaiseConfiguredError(this, new ErrorEventArgs(ex));
                    throw new InvalidConfigurationParameterException($"Exception during configuration of {Name}", ex);
                }
            }
        }

        public bool TryConfigure(VisionFunctionOptions options)
        {
            bool result = false;
            try
            {
                Configure(options);
                result = true;
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(RaiseExceptionNotification));
            }
            return result;
        }

        protected void RaiseConfigured(object sender, EventArgs<VisionFunctionOptions> eventArgs)
        {
            try
            {
                Configured?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(RaiseExceptionNotification));
            }
        }
        public event EventHandler<EventArgs<VisionFunctionOptions>> Configured;

        protected void RaiseConfiguredError(object sender, ErrorEventArgs eventArgs)
        {
            try
            {
                ConfiguredError?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(RaiseExceptionNotification));
            }
        }
        public event EventHandler<ErrorEventArgs> ConfiguredError;

        protected void _configureInputParameters(TOptions options)
        {
            var inputParams = options.Parameters.Where(p => p.Direction == ParameterDirection.Input).ToArray();
            _inputParameters.AddRange(inputParams);
        }

        protected abstract void _configure(VisionFunctionOptions options);

        public void Initialize(params (string, object)[] parameters)
        {
            if (_executionStatusTransition.IsValidTransition(Status, ExecutionStatus.Executing))
            {
                Status = ExecutionStatus.Executing;
                try
                {
                    foreach (var parameter in parameters)
                    {
                        (string paramIndex, object paramValue) = parameter;

                        if (!InputParameters.Exists(paramIndex))
                        {
                            var newEx = new ArgumentException($"Parameter {paramIndex} does not exist in {Index} vision function.");
                            RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                            throw newEx;
                        }

                        var inputParam = InputParameters.Get(paramIndex);

                        if (inputParam.Scope != ParameterScope.Initialization)
                        {
                            var newEx = new ArgumentException($"{parameter.Item1} is not a initializacion parameter in {Index} vision function.");
                            RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                            throw newEx;
                        }

                        InputParameters[paramIndex] = paramValue;
                    }

                    _initialize();
                }
                catch (Exception ex)
                {
                    Status = ExecutionStatus.Error;
                    RaiseConfiguredError(this, new ErrorEventArgs(ex));
                    throw new InvalidConfigurationParameterException($"Exception during initialization of {Name}", ex);
                }

                Status = ExecutionStatus.Finished;
            }
        }

        protected virtual void _initialize()
        {

        }

        protected void Execute()
        {
            string errorMessage = string.Empty;

            bool success = false;

            _inputParameters.Readonly = true;
            _outputParameters = new TOutputParam();
            _outputParameters.ParentName = Name;
            _outputParameters.AddRange(Options.Parameters.Where(p => p.Direction == ParameterDirection.Output).ToArray());

            bool enabled = _inputParameters.Enabled;
            bool prevResult = _inputParameters.PrevResult;

            _outputParameters.Enabled = enabled;
            _outputParameters.PrevResult = prevResult;

            if (!enabled || !prevResult)
            {
                _outputParameters.Result = false;
                _outputParameters.ProcessTime = 0d;
                _outputParameters.Success = true;
                _outputParameters.Error = string.Empty;
            }
            else
            {
                _durationMeter.Start();
                try
                {
                    success = _execute(_inputParameters, ref _outputParameters);
                    if (success)
                    {
                        success = _outputParameters.Success;
                        errorMessage = _outputParameters.Error;
                    }
                    else
                        throw new VisionFunctionExecutionException(_outputParameters.Error);
                }
                catch (Exception ex)
                {
                    success = false;
                    Status = ExecutionStatus.Error;
                    errorMessage = ex.Message;
                    RaiseExceptionNotification(this, new ErrorEventArgs(ex));
                }
                _durationMeter.Stop();
                _outputParameters.ProcessTime = _durationMeter.LastDuration.TotalMilliseconds;
                _outputParameters.Success = success;
                _outputParameters.Error = errorMessage;
                _outputParameters.PrevResult = prevResult && success;
            }

            _inputParameters.Readonly = false;
            //_outputParameters.Unlink();

            if (success)
                RaiseCompleted(this, new EventArgs<IParametersCollection>(_outputParameters));
            else
                RaiseError(this, new ErrorEventArgs(new VisionFunctionExecutionException(errorMessage)));
        }

        public void Execute(params (string, object)[] parameters)
        {
            if (_executionStatusTransition.IsValidTransition(Status, ExecutionStatus.Executing))
            {
                Status = ExecutionStatus.Executing;
                try
                {
                    foreach (var parameter in parameters)
                    {
                        (string paramIndex, object paramValue) = parameter;

                        if (!InputParameters.Exists(paramIndex))
                        {
                            var newEx = new ArgumentException($"Parameter {paramIndex} does not exist in {Index} vision function.");
                            RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                            throw newEx;
                        }

                        var inputParam = InputParameters.Get(paramIndex);

                        if (inputParam.Scope != ParameterScope.Execution)
                        {
                            var newEx = new ArgumentException($"{parameter.Item1} is not a execution parameter in {Index} vision function.");
                            RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                            throw newEx;
                        }

                        InputParameters[paramIndex] = paramValue;
                    }

                    Execute();
                }
                catch (Exception ex)
                {
                    Status = ExecutionStatus.Error;
                    RaiseConfiguredError(this, new ErrorEventArgs(ex));
                    throw new InvalidConfigurationParameterException($"Exception during initialization of {Name}", ex);
                }

                Status = ExecutionStatus.Finished;
            }
        }

        public InspectionResult Execute(IInputParametersCollection inputParameters, List<NamedValue> step)
        {
            if (_executionStatusTransition.IsValidTransition(Status, ExecutionStatus.Executing))
            {
                Status = ExecutionStatus.Executing;
                _inputParameters = inputParameters as InputParametersCollection;
                Execute();
            }

            InspectionResult inspectionResult = new InspectionResultAdapter().Convert(_outputParameters, _includeInResult, _includeInStats, _isSegmentationInspection, step);

            if(_executionStatusTransition.IsValidTransition(Status, ExecutionStatus.Finished))
            {
                // Ojo, si hay una excepción nunva vuelve a finished
                Status = ExecutionStatus.Finished;
            }

            return inspectionResult;
        }

        protected abstract bool _execute(InputParametersCollection inputParams, ref OutputParametersCollection outputParams);

        public bool TryExecute()
        {
            var success = false;
            try
            {
                Execute();
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
            return success;
        }

        public IOutputParametersCollection CreateInputParameters()
        {
            var outputParams = new TOutputParam();
            _outputParameters.ParentName = Name;
            outputParams.AddRange(Options.Parameters.Where(p => p.Direction == ParameterDirection.Input).ToArray());
            return outputParams;
        }

        public IOutputParametersCollection CreateOutputParameters()
        {
            var outputParams = new TOutputParam();
            _outputParameters.ParentName = Name;
            outputParams.AddRange(Options.Parameters.Where(p => p.Direction == ParameterDirection.Output).ToArray());
            return outputParams;
        }

        protected void RaiseExceptionNotification(object sender, ErrorEventArgs eventArgs)
        {
            try
            {
                ExceptionRaised?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(RaiseExceptionNotification));
            }
        }
        public event EventHandler<ErrorEventArgs> ExceptionRaised;

        protected void RaiseCompleted(object sender, EventArgs<IParametersCollection> eventArgs)
        {
            try
            {
                ExecutionCompleted?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }
        public event EventHandler<EventArgs<IParametersCollection>> ExecutionCompleted;

        protected void RaiseError(object sender, ErrorEventArgs eventArgs)
        {
            try
            {
                ExecutionError?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }
        public event EventHandler<ErrorEventArgs> ExecutionError;

        protected void RaiseExecutionStatusChanged(object sender, EventArgs<ExecutionStatus> eventArgs)
        {
            try
            {
                ExecutionStatusChanged?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }
        public event EventHandler<EventArgs<ExecutionStatus>> ExecutionStatusChanged;

        public override string ToString()
        {
            return $"Vision function of type: {GetType().Name}, Description: {Description}, Status: {Status}";
        }

        public abstract IInputParametersCollection GetInputParameters(VisionMessage vm);

        public virtual IOutputParametersCollection NewEmptyOutputParameters(bool enabled, bool prevResult, bool result, bool success, bool warning, string error)
        {
            var outputParameters = CreateOutputParameters() as OutputParametersCollection;
            outputParameters.Enabled = enabled;
            outputParameters.PrevResult = prevResult;
            outputParameters.Result = result;
            outputParameters.Success = success;
            outputParameters.Error = error;
            outputParameters.Warning = warning;
            outputParameters.ParentName = Name;
            outputParameters.ProcessTime = 0.0;

            return outputParameters;
        }

        public virtual InspectionResult NewEmptyInspectionResult(bool enabled, bool prevResult, bool result, bool success, bool warning, string error, List<NamedValue> step)
        {
            // Resultado vacío
            var outputParameters = NewEmptyOutputParameters(enabled, prevResult, result, success, warning, error);

            InspectionResult inspectionResult = new InspectionResultAdapter().Convert(outputParameters, _includeInResult, _includeInStats, _isSegmentationInspection, step);

            return inspectionResult;
        }

        #region IPoolable

        public bool IsOccupied { get; protected set; } = false;

        public void Hold()
        {
            IsOccupied = true;
        }

        public void Release()
        {
            IsOccupied = false;
        }

        #endregion IPoolable

        #region Runtime Parameters Methods

        public void RefreshRuntimeParameters(VisionFunctionOptions options)
        {
            RefreshRuntimeParameters((TOptions)options);
        }

        public virtual void RefreshRuntimeParameters(TOptions options)
        {
            if (Status == ExecutionStatus.Executing)
            {
                // Si está en ejecución, se suscribe al evento y espera el cambio
                EventHandler<EventArgs<ExecutionStatus>> handler = null;
                handler = (sender, args) =>
                {
                    // Cuando el estado ya no sea Executing
                    if (args.Value != ExecutionStatus.Executing)
                    {
                        // Se desuscribe para evitar llamadas múltiples
                        this.ExecutionStatusChanged -= handler;
                        Options = options;
                        _setRuntimeParameters(options);
                    }
                };

                this.ExecutionStatusChanged += handler;
            }
            else
            {
                Options = options;
                _setRuntimeParameters(options);
            }
        }

        protected virtual void _setRuntimeParameters(TOptions options)
        {
        }

        #endregion
    }
}

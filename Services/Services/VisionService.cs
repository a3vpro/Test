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
using VisionNet.Core.Abstractions;
using Castle.Core.Logging;
using VisionNet.Application;
using System.Linq;
using Conditions;
using System;
using VisionNet.Core.Exceptions;
using System.Collections.Concurrent;
using System.IO;
using System.Collections.Generic;
using VisionNet.Vision.Core;
using System.Drawing;
using VisionNet.Image;
using VisionNet.Core;

namespace VisionNet.Vision.Services
{
    public class VisionService : IVisionService, IVisionResultService, IVisionPool
    {
        protected readonly ILogger _log;
        protected readonly IOptionsRepository _optionsRepository;
        protected readonly IServiceRepository _serviceRepository;

        protected VisionLoggedFactory _visionFactory;
        protected ConcurrentDictionary<string, List<IVisionFunction>> _vision = new ConcurrentDictionary<string, List<IVisionFunction>>();
        protected int _pipelinePoolSize;

        public static IVisionService Default { get; private set; }
        public object Instance => Default;

        public string Name { get; protected set; } = nameof(VisionService);

        public string Description { get; protected set; } = "Vision service";

        public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;

        public virtual VisionOptions Options { get; protected set; } = new VisionOptions();

        public VisionService(ILogger log = null, IOptionsRepository optionsRepository = null, IServiceRepository serviceRepository = null)
        {
            _log = log;
            _optionsRepository = optionsRepository;
            _serviceRepository = serviceRepository;

            if (Default == null)
                Default = this;

            _visionFactory = new VisionLoggedFactory(_serviceRepository, _log);
        }

        #region Read Only Repository

        /// <summary>
        /// Obtiene el número de máquinas de estados gestionadas por el servicio.
        /// </summary>
        /// <returns>El número de máquinas de estados gestionadas.</returns>
        public int Count()
        {
            return _vision.Count();
        }

        /// <summary>
        /// Comprueba si una función de visión existe en el servicio.
        /// </summary>
        /// <param name="id">El identificador de la función de visión.</param>
        /// <returns>Verdadero si la función de visión existe, falso en caso contrario.</returns>
        public bool Exists(string id)
        {
            return _vision.ContainsKey(id);
        }

        /// <summary>
        /// Obtiene una máquina de estados por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la máquina de estados.</param>
        /// <returns>La máquina de estados correspondiente al identificador.</returns>
        public List<IVisionFunction> Get(string id)
        {
            if (!Exists(id))
            {
                var newEx = new ArgumentException($"{id} not found in existing StateMachine. Try use Get<T> instead.");
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }

            return _vision[id];
        }

        /// <summary>
        /// Obtiene una máquina de estados por su identificador.
        /// </summary>
        /// <typeparam name="T">El tipo de la máquina de estados.</typeparam>
        /// <param name="id">El identificador de la máquina de estados.</param>
        /// <returns>La máquina de estados correspondiente al identificador.</returns>
        public T Get<T>(string id) where T : List<IVisionFunction>, new()
        {
            if (!Exists(id))
            {
                var newEx = new ArgumentException($"{id} not found in existing StateMachine. Try use Get<T> instead.");
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }

            return (T)Get(id);
        }

        /// <summary>
        /// Obtiene todas las máquinas de estados gestionadas por el servicio.
        /// </summary>
        /// <returns>Una lista de todas las máquinas de estados gestionadas.</returns>
        public IList<List<IVisionFunction>> GetAll()
        {
            return _vision.Values.ToList();
        }

        #endregion

        public virtual void Setup()
        {
            try
            {
                // Start Services
                var options = _optionsRepository.TryGet(nameof(VisionOptions), VisionOptions.DefaultInstance);

                options.PipelinePoolSize.Requires(nameof(options.PipelinePoolSize)).IsGreaterThan(0);
                _pipelinePoolSize = options.PipelinePoolSize;

                options.VisionFunctions.Select(c => string.IsNullOrWhiteSpace(c.Index))
                    .Requires(nameof(options.VisionFunctions))
                    .DoesNotContain(true);
                options.VisionFunctions.GroupBy(x => x.Index).Where(g => g.Count() > 1)
                    .Requires(nameof(options.VisionFunctions))
                    .IsEmpty();
                options.VisionFunctions.Select(c => string.IsNullOrWhiteSpace(c.Name))
                    .Requires(nameof(options.VisionFunctions))
                    .DoesNotContain(true);

                foreach (VisionFunctionOptions visionOption in options.VisionFunctions)
                {
                    List<IVisionFunction> lstFunctions = new List<IVisionFunction>();
                    for(int i = 0; i < visionOption.MaxDegreeOfParallelism; i++)
                    {
                        _visionFactory.VisionFunctionType = visionOption.VisionFunctionType;
                        _visionFactory.VisionFunctionName = visionOption.Index;
                        IVisionFunction vision = _visionFactory.Create();
                        vision.Configure(visionOption);

                        lstFunctions.Add(vision);
                    }

                    _vision[visionOption.Index] = lstFunctions;
                }

                Options = options;
            }
            catch (Exception ex)
            {
                var newEx = new InvalidConfigurationParameterException($"Exception during configuration of {nameof(VisionService)}", ex);
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }
        }

        public void Start()
        {
            if (Status == ServiceStatus.Stopped)
            {
                try
                {
                    _log?.Info($"Starting {Description}");

                    foreach (var vision in _vision.Values)
                    {
                        foreach (var visionFunction in vision)
                        {
                            visionFunction.ExceptionRaised += RaiseExceptionNotification;
                        }
                    }

                    _log?.Info($"Started {Description}");
                }
                catch (Exception ex)
                {
                    _log?.Fatal($"Exception during starting {Description}", ex);
                }
                Status = ServiceStatus.Started;
            }
        }

        public void Stop()
        {
            if (Status == ServiceStatus.Started)
            {
                try
                {
                    _log?.Info($"Stopping {Description}");

                    foreach (var vision in _vision.Values)
                    {
                        foreach (var visionFunction in vision)
                        {
                            visionFunction.ExceptionRaised -= RaiseExceptionNotification;
                        }
                    }

                    _log?.Info($"Stopped {Description}");
                }
                catch (Exception ex)
                {
                    _log?.Error($"Exception during stopping {Description}", ex);
                }

                Status = ServiceStatus.Stopped;
            }
        }

        public IVisionFunction GetFromPool(string index) // La responsabilidad del que llama a esta función es utilizarla y devolverla!!! Haría falta un BackToPool()
        {
            if (!Exists(index))
            {
                var newEx = new ArgumentException($"{index} not found in existing Vision. Try use Get<T> instead.");
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }

            return _getAvailableFunction(index);
        }

        private IVisionFunction _getAvailableFunction(string index)
        {
            IVisionFunction visionFunction = _vision[index].FirstOrDefault(v => !v.IsOccupied);
            visionFunction.Hold(); // Aquí se hace el hold pero en otro lugar muy muy lejano se hace el release!
            return visionFunction;
        }

        /// <inheritdoc/>
        public void Init(ProductFeatures productFeatures)
        {
            RaiseNewProductIsReadyToInspect(this, new VisionProductFeaturesEventArgs(productFeatures));
        }

        public void Post(ProductResult productResult, DateTime dateTime = default)
        {
            RaiseNewProductResult(this, new VisionResultEventArgs(productResult, dateTime));
        }
        /// <inheritdoc/>
        public void PostInspection(InspectionResult productResult, DateTime dateTime = default)
        {
            RaiseNewInspection(this, new VisionInspectionEventArgs(productResult, dateTime));
        }

        #region Events

        protected void RaiseNewProductIsReadyToInspect(object sender, VisionProductFeaturesEventArgs eventArgs)
        {
            try
            {
                NewProductIsReadyToInspect?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }
        /// <inheritdoc/>
        public event EventHandler<VisionProductFeaturesEventArgs> NewProductIsReadyToInspect;

        protected void RaiseNewProductResult(object sender, VisionResultEventArgs eventArgs)
        {
            try
            {
                NewProductResult?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }
        /// <inheritdoc/>
        public event EventHandler<VisionResultEventArgs> NewProductResult;

        protected void RaiseNewInspection(object sender, VisionInspectionEventArgs eventArgs)
        {
            try
            {
                NewInspection?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }
        /// <inheritdoc/>
        public event EventHandler<VisionInspectionEventArgs> NewInspection;

        protected void RaiseExceptionNotification(object sender, ErrorEventArgs eventArgs)
        {
            try
            {
                _log?.Error($"Error in {Description}", eventArgs.GetException());
                ExceptionRaised?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(RaiseExceptionNotification));
            }
        }
        /// <inheritdoc/>
        public event EventHandler<ErrorEventArgs> ExceptionRaised;
        #endregion

        #region Initialization
        public void InitVisionFunction(string index, params (string, object)[] parameters)
        {
            if (!Exists(index))
            {
                var newEx = new ArgumentException($"{index} not found in existing Vision. Try use Get<T> instead.");
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }

            if (_vision[index].Any(v => v.IsOccupied))
            {
                var newEx = new ArgumentException($"Any vision funcion of {index} are busy. Wait to be freezed.");
                RaiseExceptionNotification(this, new ErrorEventArgs(newEx));
                throw newEx;
            }

            var visionFunctions = _vision[index];
            visionFunctions.ForEach(v =>
            {
                v.Hold();
                v.Initialize(parameters);
                v.Release();
            });
        }
        #endregion

        #region Refresh All Runtime Parameters

        public void RefreshAllRuntimeParameters(Dictionary<string, IRuntimeOptions> optionsDictionary)
        {
            foreach (KeyValuePair<string, IRuntimeOptions> kvp in optionsDictionary)
            {
                // Se obtienen todas las funciones de visión asociadas a la key.
                List<IVisionFunction> items = this.Get(kvp.Key);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        // Se realiza el casting a VisionFunctionOptions para refrescar los parámetros.
                        item.RefreshRuntimeParameters(kvp.Value as VisionFunctionOptions);
                    }
                }
            }
        }

        #endregion
    }
}

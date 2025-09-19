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
using System.IO;
using System.Linq;
using Conditions;
using VisionNet.Core.Exceptions;
using VisionNet.Core.Patterns;
using VisionNet.Core.SafeObjects;
using VisionNet.Core.Types;
using VisionNet.Image;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Metadata;
using VisionNet.Core.Attributes;
namespace VisionNet.Vision.Core
{
    public abstract class ParametersCollection : SafeObjectCollection<string, IParameter, BasicTypeCode>, IParametersCollection, IExceptionObservable
    {
        /// <inheritdoc/>
        public abstract ParameterDirection Direction { get; }

        /// <inheritdoc/>
        public string ParentName { get; set; } = "No name";

        /// <inheritdoc/>
        public IReadOnlyList<IParameter> NotReservedParameters
        {
            get
            {
                // Filtra los elementos que no están en la lista ReservedParametersNames y crea un nuevo diccionario con ellos
                return this.GetAll()
                    .Where(p => !ReservedParametersNames.Contains(p.Index))
                    .ToList();
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<IParameter> ToMeasurablesParameters
        {
            get
            {
                // Filtra los elementos que no están en la lista ReservedParametersNames y crea un nuevo diccionario con ellos
                return this.GetAll()
                    .Where(p => ToMeasurablesParametersNames.Contains(p.Index))
                    .ToList();
            }
        }

        public IReadOnlyList<IReadonlyParameter> NotReservedReadonlyParameters
        {
            get
            {
                // Filtra los elementos que no están en la lista ReservedParametersNames y crea un nuevo diccionario con ellos
                return this.GetAll()
                    .Where(p => !ReservedParametersNames.Contains(p.Index))
                    .ToList();
            }
        }

        public IReadOnlyList<IReadonlyParameter> ToMeasurablesReadonlyParameters
        {
            get
            {
                // Filtra los elementos que no están en la lista ReservedParametersNames y crea un nuevo diccionario con ellos
                return this.GetAll()
                    .Where(p => ToMeasurablesParametersNames.Contains(p.Index))
                    .ToList();
            }
        }

        /// <inheritdoc/>
        public bool Readonly { get; set; }

        /// <summary>
        /// List of all parameters name that have a direct implementation as a property.
        /// </summary>
        protected List<string> ReservedParametersNames { get; private set; } = new List<string>();

        /// <summary>
        /// List of all parameters name that should be stored for persistence.
        /// </summary>
        protected List<string> ToMeasurablesParametersNames { get; private set; } = new List<string>();

        public ParametersCollection()
        {
        	// Cargar las propiedades marcadas con ParameterAttribute de la propia instancia
            var propertiesWithAttributes = this.GetFirstPropertyWithAttribute<ParameterAttribute>();
            foreach (var propertyWithAttribute in propertiesWithAttributes)
            {
                TryAdd(propertyWithAttribute.Value.ParameterOptions);
                ReservedParametersNames.Add(propertyWithAttribute.Value.ParameterOptions.Name);

                if (propertyWithAttribute.Value.ParameterOptions.Direction == ParameterDirection.Output
                    && propertyWithAttribute.Value.ParameterOptions.IncludeInMeasurable)
                    ToMeasurablesParametersNames.Add(propertyWithAttribute.Value.ParameterOptions.Name);
            }
        }

        /// <inheritdoc/>

        public IParameter GetModificable(string id)
        {
            _values.TryGetValue(id, out var entity);
            return entity;
        }

        /// <inheritdoc/>
        public IList<IParameter> GetAllModificable()
        {
            return _values.Values.ToList();
        }

        /// <inheritdoc/>
        public override bool TryGetValue(string key, out object value)
        {
            bool success = base.TryGetValue(key, out value);
            if (!success)
            {
                var ex = new ArgumentException($"Key {key} not found in parameters collection");
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
            return success;
        }

        /// <inheritdoc/>
        public override bool TrySetValue(string key, object value)
        {
            if (Readonly)
                throw new BloquedParameterException();
            bool success = base.TrySetValue(key, value);
            if (!success)
            {                
                var ex = new ArgumentException($"Key {key} not found in parameters collection");
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
            return success;
        }

        /// <inheritdoc/>
        public IImage ToImage(string key, IImage defaultValue = null)
        {
            return _values.TryGetValue(key, out var safeObject)
                ? safeObject.ToImage(defaultValue)
                : defaultValue;
        }

        ///// <inheritdoc/>
        //public bool TrySetParmValue(string key, object value)
        //{
        //    bool success = false;
        //    try
        //    {
        //        if (Readonly)
        //            throw new BloquedParameterException();

        //        success = TryGetValue(key, out IReadonlyParameter safeObjectValue);
        //        success = safeObjectValue is IParameter;
        //        if (success)
        //            success = (safeObjectValue as IParameter).TrySetValue(value);
        //    }
        //    catch (Exception ex)
        //    {
        //        RaiseExceptionNotification(this, new ErrorEventArgs(ex));
        //    }

        //    return success;
        //}

        internal bool TryAdd(ParameterOptions options)
        {
            var result = false;
            try
            {
                options.Requires(nameof(options)).IsNotNull();
                if (Readonly)
                    throw new BloquedParameterException();
                if (options.Direction != options.Direction)
                    throw new NotAllowedParameterException($"Parameter {options.Name} must be an {Direction}");

                // If parameter does not exist
                if (!this.Exists(options.Name))
                {
                    var parameter = new Parameter(options);
                    parameter.ParentName = ParentName;
                    parameter.ExceptionRaised += RaiseExceptionNotification;

                    result = _values.TryAdd(parameter.Index, parameter);
                }
                else // If parameter exists
                {
                    Parameter existingParameter = Get(options.Name) as Parameter;
                    existingParameter.SaveToResult = options.SaveToResult;
                    existingParameter.IncludeInMeasurable = options.IncludeInMeasurable;
                    existingParameter.SetDefaultValue(options.DefaultValue);
                    result = TrySetValue(options.Name, existingParameter.DefaultValue);
                }            
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
                throw;
            }
            return result;
        }

        internal void AddRange(params ParameterOptions[] options)
        {
            try
            {
                options.Requires(nameof(options)).IsNotNull();
                if (Readonly)
                    throw new BloquedParameterException();

                var success = true;
                foreach (var parameter in options)
                    //if(!ReservedParametersNames.Contains(parameter.Name)) //ALV: Revisar con Dani
                        success = TryAdd(parameter) && success;

                if (!success)
                    throw new ArgumentException($"Imposible to add a new parameter");
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
                throw;
            }
        }

        public bool TryAdd(IReadonlyParameter parameter)
        {
            var result = false;
            try
            {
                parameter.Requires(nameof(parameter)).IsNotNull();

                result = _values.TryAdd(parameter.Index, parameter as IParameter);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
            return result;
        }

        public bool TryRemove(string parameterIndex)
        {
            var result = false;
            try
            {
                parameterIndex.Requires(nameof(parameterIndex)).IsNotNull();

                result = _values.TryRemove(parameterIndex, out IParameter removedParameter);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
            return result;
        }

        public void AddRange(params IReadonlyParameter[] parameters)
        {
            try
            {
                parameters.Requires(nameof(parameters)).IsNotNull();
                if (Readonly)
                    throw new BloquedParameterException();

                var success = true;
                foreach (var parameter in parameters)
                {
                    if (!ReservedParametersNames.Contains(parameter.Name))
                        success = TryAdd(parameter) && success;
                }

                if (!success)
                    throw new ArgumentException($"Imposible to add a new parameter");
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
                throw;
            }
        }

        /// <inheritdoc/>
        IReadonlyParameter IReadOnlyRepository<IReadonlyParameter, string>.Get(string id)
        {
            return base.Get(id);
        }

        /// <inheritdoc/>
        IList<IReadonlyParameter> IReadOnlyRepository<IReadonlyParameter, string>.GetAll()
        {
            return base.GetAll().Cast<IReadonlyParameter>().ToList();
        }

        /// <summary>
        /// Raises an exception notification event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The event arguments containing the exception.</param>
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

        /// <inheritdoc/>
        public event EventHandler<ErrorEventArgs> ExceptionRaised;
    }
}

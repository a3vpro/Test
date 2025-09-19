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
using Conditions;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Exceptions;
using VisionNet.Core.Patterns;
using VisionNet.Core.SafeObjects;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Represents a parameter of a vision funcion within the VisionNet framework, encapsulating various aspects such as name, index, and type, along with its intended direction, source, and scope.
    /// </summary>
    public class Parameter: VisionSafeObject, IParameter, IExceptionObservable, ICloneable<Parameter>
    {
        //TODO: Generar propiedad Value heredada
        /// <inheritdoc/>
        public string Index { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; } = "No name";

        /// <inheritdoc/>
        public string ExternalIndex { get; set; } = "Unkwnown";

        /// <inheritdoc/>
        public string ParentName { get; set; } = "No name";

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>   º
        public ParameterDirection Direction { get; set; }

        /// <inheritdoc/>
        public ParameterSource Source { get; set; }

        /// <inheritdoc/>
        public ParameterScope Scope { get; set; }

        /// <inheritdoc/>
        public bool Readonly { get; internal set; }

        /// <inheritdoc/>
        public bool IncludeInMeasurable { get; internal set; }

        /// <inheritdoc/>
        public bool SaveToResult { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class with specified properties.
        /// </summary>
        /// <param name="index">The index of the parameter.</param>
        /// <param name="externalIndex">The external reference index.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="description">The description of the parameter.</param>
        /// <param name="direction">The intended direction (input/output) of the parameter.</param>
        /// <param name="source">The source of the parameter value (fixed/runtime).</param>
        /// <param name="scope">The scope or intended use of the parameter.</param>
        /// <param name="dataType">The data type of the parameter.</param>
        /// <param name="isArray">Indicates if the parameter is an array.</param>
        /// <param name="defaultValue">The default value of the parameter.</param>
        /// <param name="preferences">Type conversion preferences.</param>
        /// <param name="includeInMeasurable">The parameter will be marked to be included in the product result</param>
        /// <param name="saveToResult">The parameter will be marked to be included in results</param>
        public Parameter(string index, string externalIndex, string name, string description, ParameterDirection direction, ParameterSource source, ParameterScope scope, BasicTypeCode dataType, bool isArray, object defaultValue = null, TypeConversionPreferences preferences = TypeConversionPreferences.None, bool includeInMeasurable = false, bool saveToResult = false)
            : base(dataType, isArray, defaultValue, preferences)
        {
            index.Requires(nameof(index)).IsNotNullOrWhiteSpace();
            name.Requires(nameof(name)).IsNotNullOrWhiteSpace();

            Index = index;
            ExternalIndex = externalIndex;
            Name = name;
            Description = description;
            Source = source;
            Scope = scope;
            IncludeInMeasurable = includeInMeasurable;
            SaveToResult = saveToResult;
        }

        /// <summary>
        /// Configure the instance
        /// </summary>
        /// <param name="options">Set of options to configure</param>
        /// <exception cref="InvalidConfigurationParameterException">Wrong option</exception>
        public Parameter(ParameterOptions options)
            : base(options.DataType, options.IsArray, options.DefaultValue, options.TypeConversionPreferences)
        {
            options.Requires(nameof(options)).IsNotNull();
            options.Index.Requires(nameof(options.Index)).IsNotNullOrWhiteSpace();
            options.Name.Requires(nameof(options.Name)).IsNotNullOrWhiteSpace();

            Index = options.Index;
            ExternalIndex = options.ExternalIndex;
            Name = options.Name;
            Description = options.Description;
            Source = options.Source;
            Scope = options.Scope;
            IncludeInMeasurable = options.IncludeInMeasurable;
            SaveToResult = options.SaveToResult;
        }

        /// <inheritdoc/>
        public override bool TrySetValue(object value)
        {
            bool canConvert = false;

            if (Readonly)
            {
                var ex = new BloquedParameterException($"Attempt to modify the value of a locked parameter named {Name}");
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
            else
            {
                try
                {
                    lock (_lockObject)
                    {
                        canConvert = value.TryChangeType(DataType, IsArray, out var tmpValue, DefaultValue, Preferences);

                        if (canConvert)
                            _value = tmpValue;
                    }
                }
                catch (Exception ex)
                {
                    RaiseExceptionNotification(this, new ErrorEventArgs(ex));
                }
            }
            return canConvert;
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

        /// <summary>
        /// Occurs when an exception is raised.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ExceptionRaised;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Parameter of type: {GetType().Name}, Description: {Description}, Index: {Index}";
        }

        /// <inheritdoc/>
        public Parameter Clone()
        {
            var result = new Parameter(Index, ExternalIndex, Name, Description, Direction, Source, Scope, DataType, IsArray, DefaultValue, Preferences, IncludeInMeasurable, SaveToResult);

            result.TrySetValue(GetValue());

            return result;
        }

        /// <inheritdoc/>
        public IImage ToImage(IImage defaultValue = null)
        {
            var success = TryGetValue<IImage>(out var value);
            return success ? value : defaultValue;
        }
    }
}

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

namespace VisionNet.Core.Monitoring
{
    /// <summary>
    /// Represents a repository for storing and managing monitorized values.
    /// Provides methods to add, retrieve, and manage monitorized values by index.
    /// </summary>
    public class MonitorizedValuesRepository
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="MonitorizedValuesRepository"/>.
        /// </summary>
        public static MonitorizedValuesRepository Instance { get; } = new MonitorizedValuesRepository();

        private Dictionary<string, IMonitorizedValue> _monitorizedValues = new Dictionary<string, IMonitorizedValue>();

        /// <summary>
        /// Adds a new monitorized value to the repository.
        /// If a monitorized value with the same index already exists, it will be replaced.
        /// </summary>
        /// <param name="dataType">The type of the data to be stored in the monitorized value.</param>
        /// <param name="defaultValue">The default value to initialize the monitorized value with.</param>
        /// <param name="index">The index under which the monitorized value will be stored.</param>
        /// <param name="name">The name of the monitorized value.</param>
        /// <param name="description">The description of the monitorized value.</param>
        /// <returns>A reference to the monitorized value that was added.</returns>
        public void Add(TypeCode dataType, object defaultValue, string index, string name, string description)
        {
            _monitorizedValues[index] = new MonitorizedValue(dataType, defaultValue)
            {
                Index = index,
                Name = name,
                Description = description,
            };
        }

        /// <summary>
        /// Adds an existing monitorized value to the repository.
        /// If a monitorized value with the same index already exists, it will be replaced.
        /// </summary>
        /// <param name="monitorizedValue">The monitorized value to add to the repository.</param>
        /// <returns>A reference to the monitorized value that was added.</returns>
        public void Add(MonitorizedValue monitorizedValue)
        {
            _monitorizedValues[monitorizedValue.Index] = monitorizedValue;
        }

        /// <summary>
        /// Gets the monitorized value associated with the specified index.
        /// If the index does not exist, it returns null.
        /// </summary>
        /// <param name="index">The index of the monitorized value to retrieve.</param>
        /// <returns>The monitorized value associated with the given index, or null if not found.</returns>
        public IMonitorizedValue this[string index]
        {
            get
            {
                var found = _monitorizedValues.TryGetValue(index, out var monitorizedValue);
                return found ? monitorizedValue : null;
            }
        }

        /// <summary>
        /// Attempts to get the monitorized value associated with the specified index.
        /// Returns true if the value exists, and sets the output parameter to the value; otherwise, returns false.
        /// </summary>
        /// <param name="index">The index of the monitorized value to retrieve.</param>
        /// <param name="monitorizedValue">The monitorized value associated with the given index, if found.</param>
        /// <returns>True if the monitorized value was found; otherwise, false.</returns>
        public bool TryGet(string index, out IMonitorizedValue monitorizedValue)
        {
            return _monitorizedValues.TryGetValue(index, out monitorizedValue);
        }

        /// <summary>
        /// Gets the monitorized value associated with the specified index.
        /// Throws a <see cref="KeyNotFoundException"/> if the index does not exist.
        /// </summary>
        /// <param name="index">The index of the monitorized value to retrieve.</param>
        /// <returns>The monitorized value associated with the given index.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the specified index is not found in the repository.</exception>
        public IMonitorizedValue Get(string index)
        {
            var success = _monitorizedValues.TryGetValue(index, out var monitorizedValue);
            if (!success)
                throw new KeyNotFoundException($"Monitorized value {index} not found");
            return monitorizedValue;
        }
    }

}

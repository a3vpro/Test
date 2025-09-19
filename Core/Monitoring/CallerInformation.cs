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
using VisionNet.Core.Patterns;
using VisionNet.Core.Tags;
using VisionNet.Core.Types;

namespace VisionNet.Core.Monitoring
{
    /// <summary>
    /// Represents information about the caller, including method, class, file name, and other related details.
    /// </summary>
    public class CallerInformation : IEntity<object>, IDescriptible
    {
        /// <summary>
        /// Gets the index of the caller in the list.
        /// </summary>
        public object Index { get; private set; }

        /// <summary>
        /// Gets the description of the caller.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the name of the class that called this function.
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        /// Gets the name of the method that called this function.
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Gets the name of the file that contains the caller.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the line number of the caller.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the tags associated with the caller, providing additional information.
        /// </summary>
        public IReadonlyTaggable<NamedValue> Tags { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallerInformation"/> class.
        /// </summary>
        /// <param name="index">The index of the caller in the list.</param>
        /// <param name="description">A description of the caller.</param>
        /// <param name="className">The name of the class that called this function.</param>
        /// <param name="methodName">The name of the method that called this function.</param>
        /// <param name="fileName">The name of the file that contains the caller.</param>
        /// <param name="lineNumber">The line number of the caller.</param>
        /// <param name="tags">The additional information of the caller.</param>
        public CallerInformation(object index = null, string description = "", string className = "", string methodName = "", string fileName = "", int lineNumber = 0, IReadonlyTaggable<NamedValue> tags = null)
        {
            Index = index;
            Description = description;
            ClassName = className;
            MethodName = methodName;
            FileName = fileName;
            LineNumber = lineNumber;
            Tags = tags;
        }
    }
}

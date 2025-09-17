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
using System.Diagnostics;
using VisionNet.Core.Tags;
using VisionNet.Core.Types;

namespace VisionNet.Core.Monitoring
{
    /// <summary>
    /// A factory class for creating instances of the <see cref="CallerInformation"/> class.
    /// </summary>
    public static class CallerInformationFactory
    {
        /// <summary>
        /// Creates a <see cref="CallerInformation"/> object using a default stack frame (from the caller's immediate method).
        /// </summary>
        /// <returns>A <see cref="CallerInformation"/> object representing the caller's information.</returns>
        public static CallerInformation Create()
        {
            return Create(new StackFrame(2, true));
        }

        /// <summary>
        /// Creates a <see cref="CallerInformation"/> object from the provided stack frame.
        /// </summary>
        /// <param name="stackFrame">The stack frame used to retrieve the caller's information.</param>
        /// <returns>A <see cref="CallerInformation"/> object with details extracted from the given stack frame.</returns>
        public static CallerInformation Create(StackFrame stackFrame)
        {
            return new CallerInformation(
                index: 0,
                description: string.Empty,
                className: stackFrame.GetMethod().DeclaringType.Name,
                methodName: stackFrame.GetMethod().Name,
                fileName: stackFrame.GetFileName(),
                lineNumber: stackFrame.GetFileLineNumber());
        }

        /// <summary>
        /// Creates a <see cref="CallerInformation"/> object using the provided index, description, and an optional set of tags.
        /// Uses reflection to gather method and class information from the caller's stack frame.
        /// </summary>
        /// <param name="index">The index of the caller, typically used to uniquely identify the caller in a sequence.</param>
        /// <param name="description">A description providing additional context or information about the caller.</param>
        /// <param name="tags">Optional tags that can provide further metadata about the caller.</param>
        /// <returns>A <see cref="CallerInformation"/> object containing the caller's information and any provided metadata.</returns>
        public static CallerInformation Create(object index, string description, IReadonlyTaggable<NamedValue> tags = null)
        {
            var stackFrame = new StackFrame(2, true);

            return new CallerInformation(
                index: index,
                description: description,
                className: stackFrame.GetMethod().DeclaringType.Name,
                methodName: stackFrame.GetMethod().Name,
                fileName: stackFrame.GetFileName(),
                lineNumber: stackFrame.GetFileLineNumber(),
                tags: tags);
        }
    }
}


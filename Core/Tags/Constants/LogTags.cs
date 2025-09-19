using System;
using System.Collections.Generic;
using System.Text;
using VisionNet.Core.Types;

namespace VisionNet.Core.Tags
{
    /// <summary>
    /// Provides the tag definitions that can be attached to log entries in order to
    /// control how they are processed throughout the logging subsystem.
    /// </summary>
    public static class LogTags
    {
        /// <summary>
        /// Tag applied to a log entry to explicitly mark it for inclusion in persisted
        /// or aggregated log outputs, even when the default logging configuration would
        /// omit it. Consumers should attach this tag to diagnostic events that must never
        /// be filtered out so downstream monitoring tools can rely on their presence.
        /// </summary>
        public static readonly NamedValue INCLUDE_IN_LOG = new NamedValue
        {
            Name = "IncludeInLogs",
            Type = BasicTypeCode.Boolean,
            Value = true,
        };
    }
}

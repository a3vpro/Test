using System;
using System.Collections.Generic;
using System.Text;
using VisionNet.Core.Types;

namespace VisionNet.Core.Tags
{
    public static class LogTags
    {
        public static readonly NamedValue INCLUDE_IN_LOG = new NamedValue
        {
            Name = "IncludeInLogs",
            Type = BasicTypeCode.Boolean,
            Value = true,
        };
    }
}

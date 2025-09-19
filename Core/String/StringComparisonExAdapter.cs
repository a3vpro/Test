using System;
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Strings
{
    /// <summary>
    /// Clase adaptadora para convertir de StringComparisonExExtended a StringComparisonEx
    /// </summary>
    public class StringComparisonExAdapter : IAdapter<StringComparisonEx, StringComparison>
    {
        public StringComparison Convert(StringComparisonEx value)
        {
            // Usamos un switch para mapear los valores del enumerado extendido a los del simplificado
            if (value.HasFlag(StringComparisonEx.CurrentCultureIgnoreCase))
                return StringComparison.CurrentCultureIgnoreCase;
            else if (value.HasFlag(StringComparisonEx.InvariantCulture))
                return StringComparison.InvariantCulture;
            else if (value.HasFlag(StringComparisonEx.InvariantCultureIgnoreCase))
                return StringComparison.InvariantCultureIgnoreCase;
            else if (value.HasFlag(StringComparisonEx.Ordinal))
                return StringComparison.Ordinal;
            else if (value.HasFlag(StringComparisonEx.OrdinalIgnoreCase))
                return StringComparison.OrdinalIgnoreCase;
            else
                return StringComparison.CurrentCulture;
        }
    }
}

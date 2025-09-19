using System;

namespace VisionNet.Core.Strings
{
    /// <summary>
    /// Clase conversora entre alarmas de tipo StringComparisonEx a StringComparison
    /// </summary>
    public static class StringComparisonAdapterExtensions
    {
        /// <summary>
        /// Converts an extended string comparison option into the equivalent <see cref="StringComparison"/> value
        /// so that it can be used with standard .NET string comparison APIs.
        /// </summary>
        /// <param name="value">The extended comparison mode to convert. This must be a defined <see cref="StringComparisonEx"/>
        /// combination describing the culture and case comparison rules to apply.</param>
        /// <returns>The <see cref="StringComparison"/> value that best represents the culture and case-sensitivity options
        /// encapsulated by <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> cannot be translated to a supported
        /// <see cref="StringComparison"/> value.</exception>
        public static StringComparison Convert(this StringComparisonEx value)
        {
            var adapter = new StringComparisonExAdapter();
            return adapter.Convert(value);
        }
    }
}
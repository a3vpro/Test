using System;

namespace VisionNet.Core.Strings
{
    /// <summary>
    /// Clase conversora entre alarmas de tipo StringComparisonEx a StringComparison
    /// </summary>
    public static class StringComparisonAdapterExtensions
    {
        public static StringComparison Convert(this StringComparisonEx value)
        {
            var adapter = new StringComparisonExAdapter();
            return adapter.Convert(value);
        }
    }
}
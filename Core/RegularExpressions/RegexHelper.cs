using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VisionNet.Core.RegularExpressions
{
    public static class RegexHelper
    {
        /// <summary>
        /// Determines whether the provided pattern can be compiled into a regular expression by instantiating a <see cref="Regex"/>.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to validate; may be <c>null</c> or empty.</param>
        /// <returns><c>true</c> if the pattern can be compiled into a <see cref="Regex"/> instance; otherwise, <c>false</c> when the pattern is <c>null</c>, malformed, or contains unsupported constructs.</returns>
        /// <remarks>Any <see cref="ArgumentException"/> thrown during compilation (including <see cref="ArgumentNullException"/>) is caught, and the method returns <c>false</c> instead of propagating the exception.</remarks>
        public static bool IsValidRegex(string pattern)
        {
            try
            {
                var regex = new Regex(pattern);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}

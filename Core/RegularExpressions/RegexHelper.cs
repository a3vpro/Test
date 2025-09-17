using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VisionNet.Core.RegularExpressions
{
    public static class RegexHelper
    {
        public static bool IsValidRegex(string pattern)
        {
            // GUARD: Ensure the pattern argument is not null before attempting to construct the regex.
            if (pattern is null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

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

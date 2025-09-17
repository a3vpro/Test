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

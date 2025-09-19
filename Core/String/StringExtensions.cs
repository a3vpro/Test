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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VisionNet.Core.Strings
{
    /// <summary>
    /// Extension methods for the string type
    /// </summary>
    public static class StringExtensions
    {
        private static readonly List<(string, string)> VisualAmbiguitiesMap = new List<(string, string)>
        {
            ( "0", "D" ),   //0 (número cero)           D (letra D mayúscula)
            ( "0", "O" ),   //0 (número cero)           O (letra O mayúscula)
            ( "1", "l" ),   //1 (número uno)            l (letra L minúscula)
            ( "1", "L" ),   //1 (número uno)            I (letra I mayúscula)
            ( "2", "z" ),   //2 (número dos)            z (letra Z minúscula)
            ( "2", "Z" ),   //2 (número dos)            Z (letra Z mayúscula)
            ( "3", "E" ),   //3 (número tres)           E (letra E mayúscula)
            ( "4", "A" ),   //4 (número cuatro)         A (letra A mayúscula)
            ( "5", "s" ),   //5 (número cinco)          s (letra S minúscula)
            ( "5", "S" ),   //5 (número cinco)          S (letra S mayúscula)
            ( "6", "G" ),   //6 (número seis)           G (letra G mayúscula)
            ( "7", "T" ),   //7 (número siete)          T (letra T mayúscula)
            ( "8", "B" ),   //8 (número ocho)           B (letra B mayúscula)
            ( "9", "q" ),   //9 (número nueve)          q (letra Q minúscula)
            ( "C", "G" ),   //C (letra C mayúscula)     G (letra G mayúscula)
            ( "d", "cl" ),  //d (letra D minúscula)     cl (letra C seguida de letra L minúscula)
            ( "F", "E" ),   //F (letra F mayúscula)     E (letra E mayúscula)
            ( "i", "l" ),   //i (letra i minúscula)     l (letra L minúscula)
            ( "I", "l" ),   //I (letra I mayúscula)     l (letra L minúscula)
            ( "M", "N" ),   //M (letra M mayúscula)     N (letra N mayúscula)
            ( "m", "rn" ),  //m (letra M minúscula)     rn (letra R seguida de letra N minúscula)
            ( "n", "r" ),   //n (letra N minúscula)     r (letra R minúscula)
            ( "Q", "O" ),   //Q (letra Q mayúscula)     O (letra O mayúscula)
            ( "U", "V" ),   //U (letra U mayúscula)     V (letra V mayúscula)
            ( "W", "VV" ),  //W (letra W mayúscula)     VV (doble V mayúscula)
        };

        public const string UpperCaseAlphabetChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string LowerCaseAlphabetChars = "abcdefghijklmnopqrstuvwxyz";
        public const string AlphabetChars = UpperCaseAlphabetChars + LowerCaseAlphabetChars;
        public const string NumericChars = "0123456789";
        public const string AphaNumericChars = AlphabetChars + NumericChars;

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another 
        /// specified string according the type of search to use for the specified string.
        /// </summary>
        /// <param name="str">The string performing the replace method.</param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string replace all occurrences of <paramref name="oldValue"/>. 
        /// If value is equal to <c>null</c>, than all occurrences of <paramref name="oldValue"/> will be removed from the <paramref name="str"/>.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue"/> are replaced with <paramref name="newValue"/>. 
        /// If <paramref name="oldValue"/> is not found in the current instance, the method returns the current instance unchanged.</returns>
        public static string Replace(this string str,
            string oldValue, string newValue,
            StringComparison comparisonType)
        {

            // Check inputs.
            if (str == null)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentNullException(nameof(str));
            }
            if (oldValue == null)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentNullException(nameof(oldValue));
            }
            if (oldValue.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentException("String cannot be of zero length.");
            }
            if (str.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                return str;
            }


            //if (oldValue.Equals(newValue, comparisonType))
            //{
            //This condition has no sense
            //It will prevent method from replacesing: "Example", "ExAmPlE", "EXAMPLE" to "example"
            //return str;
            //}



            // Prepare string builder for storing the processed string.
            // Note: StringBuilder has a better performance than String by 30-40%.
            StringBuilder resultStringBuilder = new StringBuilder(str.Length);



            // Analyze the replacement: replace or remove.
            bool isReplacementNullOrEmpty = string.IsNullOrEmpty(newValue);



            // Replace all values.
            const int valueNotFound = -1;
            int foundAt;
            int startSearchFromIndex = 0;
            while ((foundAt = str.IndexOf(oldValue, startSearchFromIndex, comparisonType)) != valueNotFound)
            {

                // Append all characters until the found replacement.
                int charsUntilReplacment = foundAt - startSearchFromIndex;
                bool isNothingToAppend = charsUntilReplacment == 0;
                if (!isNothingToAppend)
                {
                    resultStringBuilder.Append(str, startSearchFromIndex, charsUntilReplacment);
                }



                // Process the replacement.
                if (!isReplacementNullOrEmpty)
                {
                    resultStringBuilder.Append(newValue);
                }


                // Prepare start index for the next search.
                // This needed to prevent infinite loop, otherwise method always start search 
                // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
                // and comparisonType == "any ignore case" will conquer to replacing:
                // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
                startSearchFromIndex = foundAt + oldValue.Length;
                if (startSearchFromIndex == str.Length)
                {
                    // It is end of the input string: no more space for the next search.
                    // The input string ends with a value that has already been replaced. 
                    // Therefore, the string builder with the result is complete and no further action is required.
                    return resultStringBuilder.ToString();
                }
            }


            // Append the last part to the result.
            int charsUntilStringEnd = str.Length - startSearchFromIndex;
            resultStringBuilder.Append(str, startSearchFromIndex, charsUntilStringEnd);


            return resultStringBuilder.ToString();

        }

        /// <summary> The RemoveFromEnd function removes the specified suffixes from a string.</summary>
        /// <param name="str"> The string to remove the sufix from.</param>
        /// <param name="toRemove"> The params keyword is used to specify that a method parameter will be received as an array. the params modifier can be applied only to the last parameter of a formal parameter list.</param>
        /// <returns> A string with the specified suffix removed from the end.</returns>
        public static string RemoveFromStart(this string str, params string[] toRemove)
        {
            if (str == null)
            {
                // GUARD: Prevent null dereference when processing prefixes.
                return null;
            }
            if (toRemove == null || toRemove.Length == 0)
            {
                // GUARD: Nothing to remove when no prefixes are provided.
                return str;
            }
            var result = str;

            foreach (var sufix in toRemove)
            {
                if (sufix == null)
                {
                    // GUARD: Skip null prefixes.
                    continue;
                }
                result = result._removeFromStart(sufix);
            }

            return result;
        }

        private static string _removeFromStart(this string str, string toRemove)
        {
            if (str == null)
            {
                // GUARD: Preserve null input without dereferencing.
                return null;
            }
            if (str.StartsWith(toRemove))
                return str.Substring(toRemove.Length);
            else
                return str;
        }

        /// <summary> The RemoveFromEnd function removes the specified suffixes from a string.</summary>
        /// <param name="str"> The string to remove the sufix from.</param>
        /// <param name="toRemove"> The params keyword is used to specify that a method parameter will be received as an array. the params modifier can be applied only to the last parameter of a formal parameter list.</param>
        /// <returns> A string with the specified suffix removed from the end.</returns>
        public static string RemoveFromEnd(this string str, params string[] toRemove)
        {
            if (str == null)
            {
                // GUARD: Prevent null dereference when processing suffixes.
                return null;
            }
            if (toRemove == null || toRemove.Length == 0)
            {
                // GUARD: Nothing to remove when no suffixes are provided.
                return str;
            }
            var result = str;

            foreach (var sufix in toRemove)
            {
                if (sufix == null)
                {
                    // GUARD: Skip null suffixes.
                    continue;
                }
                result = result._removeFromEnd(sufix);
            }

            return result;
        }

        private static string _removeFromEnd(this string str, string toRemove)
        {
            if (str == null)
            {
                // GUARD: Preserve null input without dereferencing.
                return null;
            }
            if (str.EndsWith(toRemove))
                return str.Substring(0, str.Length - toRemove.Length);
            else
                return str;
        }

        /// <summary> The RemoveDiacritics function removes diacritics from a string.
        /// Diacritics are special characters that modify the sound of a letter, such as an accent or cedilla.
        /// </summary>
        /// <param name="text"> The text to be normalized.</param>
        /// <returns> A string without diacritics.</returns>
        public static string RemoveDiacritics(this string text)
        {
            if (text == null)
            {
                // GUARD: Preserve null input while avoiding normalization on null.
                return null;
            }
            return string.Concat(text.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark))
                .Normalize(NormalizationForm.FormC);
        }

        public static string RemoveNonAlphanumericCharacters(this string text)
        {
            if (text == null)
            {
                // GUARD: Preserve null input without invoking regular expressions.
                return null;
            }
            // Elimina cualquier caracter que no sea alfanumérico o espacio
            text = Regex.Replace(text, @"[^\w\s]", " ");

            // Colapsa cualquier secuencia de espacios en blanco (espacios, tabulaciones, saltos de línea) a un único espacio
            text = Regex.Replace(text, @"\s+", " ");

            return text.Trim(); // Elimina espacios adicionales al principio y al final
        }

        public static string NormalizeVisualAmbiguities(this string text)
        {
            if (text == null)
            {
                // GUARD: Preserve null input while avoiding replacements on null.
                return null;
            }
            // Normaliza los espacios en blanco primero
            string normalizedText = text;

            bool modified;
            do
            {
                string beforeReplacement = normalizedText;

                // Recorre cada par clave-valor en el diccionario y reemplaza en el texto
                foreach (var pair in VisualAmbiguitiesMap)
                {
                    var (source, destiny) = pair;
                    normalizedText = normalizedText.Replace(source, destiny);
                }

                // Verifica si el texto cambió durante esta iteración
                modified = (beforeReplacement != normalizedText);
            } while (modified); // Continúa hasta que no haya más cambios

            return normalizedText;
        }

        public static string NormalizeSeparatorsCharacters(this string text)
        {
            if (text == null)
            {
                // GUARD: Preserve null input without invoking regular expressions.
                return null;
            }
            // Colapsa cualquier secuencia de espacios en blanco (espacios, tabulaciones, saltos de línea) a un único espacio
            text = Regex.Replace(text, @"\s+", " ");

            return text.Trim(); // Elimina espacios adicionales al principio y al final
        }


        public static string RemoveSeparatorsCharacters(this string text)
        {
            if (text == null)
            {
                // GUARD: Preserve null input without invoking regular expressions.
                return null;
            }
            // Colapsa cualquier secuencia de espacios en blanco (espacios, tabulaciones, saltos de línea) a un único espacio
            text = Regex.Replace(text, @"\s+", "");

            return text.Trim(); // Elimina espacios adicionales al principio y al final
        }

        /// <summary>
        /// Determines if the text is valid for a C# identifier (name of class, interface, field, propierty, method...)
        /// </summary>
        /// <param name="text">Text to validate</param>
        /// <returns></returns>
        public static bool IsValidCSharpIdentifier(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            if (!char.IsLetter(text[0]) && text[0] != '_')
                return false;
            for (int ix = 1; ix < text.Length; ++ix)
                if (!char.IsLetterOrDigit(text[ix]) && text[ix] != '_')
                    return false;
            return true;
        }

        /// <summary> The EqualsEx function is a string comparison function that ignores diacritics.
        /// For example, the EqualsEx function will return true when comparing &quot;cafe&quot; and &quot;café&quot;.&lt;/para&gt;</summary>
        /// <param name="compared1"> The first string to compare.</param>
        /// <param name="compared2"> The string to compare with the current instance.</param>
        /// <param name="comparisonType"> The type of comparison to use.</param>
        /// <returns> True if the two strings are equal, false otherwise.</returns>
        public static bool EqualsEx(this string compared1, string compared2, StringComparisonEx comparisonType)
        {
            if (ReferenceEquals(compared1, compared2))
            {
                // GUARD: Identical references (including both null) are always equal.
                return true;
            }
            if (compared1 == null || compared2 == null)
            {
                // GUARD: Ensure symmetric null handling before invoking instance comparisons.
                return false;
            }
            var ct = comparisonType.Convert();

            var valueCompared1 = compared1;
            var valueCompared2 = compared2;

            if (valueCompared1.Equals(valueCompared2, ct))
                return true;

            if (comparisonType.HasFlag(StringComparisonEx.DiacriticsIgnore))
            {
                valueCompared1 = valueCompared1.RemoveDiacritics();
                valueCompared2 = valueCompared2.RemoveDiacritics();
            }
            if (valueCompared1.Equals(valueCompared2, ct))
                return true;

            if (comparisonType.HasFlag(StringComparisonEx.NonAlphanumericIgnore))
            {
                valueCompared1 = valueCompared1.RemoveNonAlphanumericCharacters();
                valueCompared2 = valueCompared2.RemoveNonAlphanumericCharacters();
            }
            if (valueCompared1.Equals(valueCompared2, ct))
                return true;

            if (comparisonType.HasFlag(StringComparisonEx.SeparatorsIgnore))
            {
                valueCompared1 = valueCompared1.RemoveSeparatorsCharacters();
                valueCompared2 = valueCompared2.RemoveSeparatorsCharacters();
            }
            if (valueCompared1.Equals(valueCompared2, ct))
                return true;

            if (comparisonType.HasFlag(StringComparisonEx.VisualAmbiguitiesIgnore))
            {
                valueCompared1 = valueCompared1.NormalizeVisualAmbiguities();
                valueCompared2 = valueCompared2.NormalizeVisualAmbiguities();
            }
            return valueCompared1.Equals(valueCompared2, ct);
        }
    }
}

namespace VisionNet.Core.Strings
{
    //
    // Resumen:
    //     Specifies the culture, case, and sort rules to be used by certain overloads of
    //     the System.String.Compare(System.String,System.String) and System.String.Equals(System.Object)
    //     methods.
    public enum StringComparisonEx
    {
        //
        // Resumen:
        //     Compare strings using culture-sensitive sort rules and the current culture.
        CurrentCulture = 0,
        //
        // Resumen:
        //     Compare strings using culture-sensitive sort rules, the current culture, and
        //     ignoring the case of the strings being compared.
        CurrentCultureIgnoreCase = 1,
        //
        // Resumen:
        //     Compare strings using culture-sensitive sort rules and the invariant culture.
        InvariantCulture = 2,
        //
        // Resumen:
        //     Compare strings using culture-sensitive sort rules, the invariant culture, and
        //     ignoring the case of the strings being compared.
        InvariantCultureIgnoreCase = 3,
        //
        // Resumen:
        //     Compare strings using ordinal (binary) sort rules.
        Ordinal = 4,
        //
        // Resumen:
        //     Compare strings using ordinal (binary) sort rules and ignoring the case of the
        //     strings being compared.
        OrdinalIgnoreCase = 5,
        /// <summary>
        /// Ignore diacricits characters
        /// </summary>
        DiacriticsIgnore = 1 << 3,
        /// <summary>
        /// Ignore non alphanumerica characters
        /// </summary>
        NonAlphanumericIgnore = 1 << 4,
        /// <summary>
        /// Ignore visual ambiguities
        /// </summary>
        VisualAmbiguitiesIgnore = 1 << 5,
        /// <summary>
        /// Ignore seprations between words
        /// </summary>
        SeparatorsIgnore = 1 << 6,
        /// <summary>
        /// Maximun permissivity. Ignore the maximun number of variables
        /// </summary>
        MaxPermissive = InvariantCultureIgnoreCase | DiacriticsIgnore | NonAlphanumericIgnore | VisualAmbiguitiesIgnore | SeparatorsIgnore,
    }
}

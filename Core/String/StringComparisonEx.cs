namespace VisionNet.Core.Strings
{
    /// <summary>
    /// Specifies comparison behaviors that control culture sensitivity, casing rules, and optional
    /// normalization heuristics applied by string comparison helpers.
    /// </summary>
    public enum StringComparisonEx
    {
        /// <summary>
        /// Compare strings using culture-aware sorting semantics tied to the current thread culture.
        /// </summary>
        CurrentCulture = 0,
        /// <summary>
        /// Compare strings using the current culture while performing a culture-aware case-insensitive match.
        /// </summary>
        CurrentCultureIgnoreCase = 1,
        /// <summary>
        /// Compare strings using culture-sensitive rules anchored to the invariant culture for deterministic results.
        /// </summary>
        InvariantCulture = 2,
        /// <summary>
        /// Compare strings using invariant culture semantics while ignoring character casing.
        /// </summary>
        InvariantCultureIgnoreCase = 3,
        /// <summary>
        /// Compare strings using ordinal (binary) value ordering without any culture adjustments.
        /// </summary>
        Ordinal = 4,
        /// <summary>
        /// Compare strings using ordinal ordering while ignoring character casing based on invariant rules.
        /// </summary>
        OrdinalIgnoreCase = 5,
        /// <summary>
        /// Request removal of diacritic marks, such as accents, before comparison to equalize canonical forms.
        /// </summary>
        DiacriticsIgnore = 1 << 3,
        /// <summary>
        /// Request removal of punctuation and other non-alphanumeric symbols prior to evaluating equality.
        /// </summary>
        NonAlphanumericIgnore = 1 << 4,
        /// <summary>
        /// Request normalization of characters that may have visually similar glyphs to reduce ambiguity.
        /// </summary>
        VisualAmbiguitiesIgnore = 1 << 5,
        /// <summary>
        /// Request omission of whitespace and separator characters so token boundaries do not affect matches.
        /// </summary>
        SeparatorsIgnore = 1 << 6,
        /// <summary>
        /// Combine invariant culture comparison with all optional normalization heuristics for the broadest match criteria.
        /// </summary>
        MaxPermissive = InvariantCultureIgnoreCase | DiacriticsIgnore | NonAlphanumericIgnore | VisualAmbiguitiesIgnore | SeparatorsIgnore,
    }
}

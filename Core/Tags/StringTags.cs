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
using System.Linq;

namespace VisionNet.Core.Tags
{
    /// <summary>
    /// Maintains a collection of string tags while normalizing all stored values to lowercase for case-insensitive lookups.
    /// </summary>
    public class StringTags: Tags<string>, IStringTaggable
    {
        /// <summary>
        /// Replaces the current tag list with the entries produced by splitting the supplied raw tag string.
        /// Every extracted tag is normalized to lowercase to provide case-insensitive comparisons.
        /// </summary>
        /// <param name="tag">Raw text containing one or more tags separated by <paramref name="separator" />.</param>
        /// <param name="separator">Character used to delimit individual tags within <paramref name="tag" />.</param>
        public void AddRawTag(string tag, char separator)
        {
            _tagList = tag.Split(separator).Select(t => t.ToLower()).ToList();
        }


        /// <summary>
        /// Adds a single tag to the collection after normalizing it to lowercase, ignoring duplicates.
        /// </summary>
        /// <param name="tag">Tag text to add. The value is normalized to lowercase before storage.</param>
        public override void AddTag(string tag)
        {
            var strTag = tag.ToLower();
            if (!_tagList.Contains(strTag))
                _tagList.Add(strTag);
        }


        /// <summary>
        /// Adds multiple tags to the collection, normalizing each value to lowercase and avoiding duplicates.
        /// </summary>
        /// <param name="tags">Tag values to add. Each entry is normalized to lowercase before storage.</param>
        public override void AddTags(params string[] tags)
        {
            var strTags = tags.Select(t => t.ToLower());

            foreach (var strTag in strTags)
                if (!_tagList.Contains(strTag))
                    _tagList.Add(strTag);
        }


        /// <summary>
        /// Determines whether every provided tag is present in the current collection after lowercase normalization.
        /// </summary>
        /// <param name="tags">Tags to verify. Each entry is normalized to lowercase before comparison.</param>
        /// <returns><see langword="true" /> when all normalized tags exist in the collection; otherwise, <see langword="false" />.</returns>
        public override bool HasAllTags(params string[] tags)
        {
            return tags.All(t => _tagList.Contains(t.ToLower()));
        }


        /// <summary>
        /// Determines whether at least one of the provided tags exists in the current collection after lowercase normalization.
        /// </summary>
        /// <param name="tags">Tags to verify. Each entry is normalized to lowercase before comparison.</param>
        /// <returns><see langword="true" /> when any normalized tag exists in the collection; otherwise, <see langword="false" />.</returns>
        public override bool HasAnyTag(params string[] tags)
        {
            return tags.Any(t => _tagList.Contains(t.ToLower()));
        }


        /// <summary>
        /// Determines whether the specified tag exists in the collection after normalization to lowercase.
        /// </summary>
        /// <param name="tag">Tag to verify. The value is normalized to lowercase before comparison.</param>
        /// <returns><see langword="true" /> when the normalized tag exists in the collection; otherwise, <see langword="false" />.</returns>
        public override bool HasTag(string tag)
        {
            return _tagList.Contains(tag.ToLower());
        }


        /// <summary>
        /// Removes the specified tag from the collection after normalizing it to lowercase.
        /// </summary>
        /// <param name="tag">Tag to remove. The value is normalized to lowercase before removal.</param>
        public override void RemoveTag(string tag)
        {
            _tagList.Remove(tag.ToLower());
        }


        /// <summary>
        /// Removes each supplied tag from the collection after normalizing the values to lowercase.
        /// </summary>
        /// <param name="tags">Tags to remove. Each entry is normalized to lowercase before comparison.</param>
        public override void RemoveTags(params string[] tags)
        {
            var strTags = tags.Select(t => t.ToLower());
            _tagList.RemoveAll(t => strTags.Contains(t));
        }
    }
}

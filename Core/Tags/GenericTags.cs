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
using VisionNet.Core.Types;

namespace VisionNet.Core.Tags
{
    public class GenericTags : Tags<NamedValue>
    {
        /// <summary> The AddTag function adds a tag to the _tagList list if it does not already exist.</summary>
        /// <param name="tag"> The tag to be added.</param>
        /// <returns> A list of tags</returns>
        public override void AddTag(NamedValue tag)
        {
            if (!_tagList.Contains(tag))
                _tagList.Add(tag);
        }


        /// <summary> The AddTags function adds tags to the _tagList list.</summary>
        /// <param name="tags"> The params keyword is used to specify that a method parameter should be treated as an array. 
        /// </param>
        /// <returns> A list of strings.</returns>
        public override void AddTags(params NamedValue[] tags)
        {
            foreach (var tag in tags)
                if (!_tagList.Contains(tag))
                    _tagList.Add(tag);
        }


        /// <summary> The HasAllTags function checks if the current object has all of the tags passed in as parameters.</summary>
        /// <param name="tags"> The tags to check for.</param>
        /// <returns> A boolean value. it returns true if all of the tags are present in the tag list.</returns>
        public override bool HasAllTags(params NamedValue[] tags)
        {
            return tags.All(t => _tagList.Contains(t));
        }


        /// <summary> The HasAnyTag function checks if the current object has any of the tags passed in as parameters.</summary>
        /// <param name="tags"> The tags to check for</param>
        /// <returns> A boolean value.</returns>
        public override bool HasAnyTag(params NamedValue[] tags)
        {
            return tags.Any(t => _tagList.Contains(t));
        }

        
        /// <summary> The HasTag function checks if the tag is in the list of tags.</summary>
        /// <param name="tag"> The tag to be added.</param>
        /// <returns> True if the tag is contained in the _taglist, otherwise it returns false.</returns>
        public override bool HasTag(NamedValue tag)
        {
            return _tagList.Contains(tag);
        }

        
        /// <summary> The RemoveTag function removes a tag from the list of tags.</summary>
        /// <param name="tag"> The tag to be removed</param>
        /// <returns> A boolean value.</returns>
        public override void RemoveTag(NamedValue tag)
        {
            _tagList.Remove(tag);
        }

        
        /// <summary> The RemoveTags function removes all tags from the tag list that match any of the strings passed in as parameters.</summary>
        /// <param name="tags"> The tags to remove from the list.</param>
        /// <returns> The list of tags that were removed.</returns>
        public override void RemoveTags(params NamedValue[] tags)
        {
            _tagList.RemoveAll(t => tags.Contains(t));
        }
    }
}

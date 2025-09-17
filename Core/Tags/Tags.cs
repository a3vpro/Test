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
using System.Linq;

namespace VisionNet.Core.Tags
{
    public class Tags<T> : ITaggable<T>, IReadonlyTaggable<T>
    {
        protected List<T> _tagList = new List<T>();

        
        /// <summary> The AddTag function adds a tag to the list of tags if it is not already in the list.</summary>
        /// <param name="tag"> The tag to be removed from the list</param>
        /// <returns> A bool, true if the tag was added and false otherwise.</returns>
        public virtual void AddTag(T tag)
        {
            if (!_tagList.Contains(tag))
                _tagList.Add(tag);
        }

        
        /// <summary> The AddTags function adds tags to the tag list.</summary>
        /// <param name="tags"> The tags to be added</param>
        /// <returns> A list of tags.</returns>
        public virtual void AddTags(params T[] tags)
        {
            foreach (var tag in tags)
                if (!_tagList.Contains(tag))
                    _tagList.Add(tag);
        }

        
        /// <summary> The HasAllTags function checks if the current object has all of the tags passed in as parameters.</summary>
        /// <param name="tags"> The params keyword is a modifier that indicates that the method parameter 
        /// list is an array of arguments. when calling a method, you can specify 
        /// arguments as individual parameters or as an array. if you use the params 
        /// keyword, then all of the following are valid:</param>
        /// <returns> True if the object has all of the tags in the array.</returns>
        public virtual bool HasAllTags(params T[] tags)
        {
            return tags.All(t => _tagList.Contains(t));
        }

        
        /// <summary> The HasAnyTag function checks if the tag list contains any of the tags passed in as parameters.</summary>
        /// <param name="tags"> The tags to check for.</param>
        /// <returns> True if the tag is present in the list.</returns>
        public virtual bool HasAnyTag(params T[] tags)
        {
            return tags.Any(t => _tagList.Contains(t));
        }

        
        /// <summary> The HasTag function checks if the tag is in the list of tags.</summary>
        /// <param name="tag"> The tag to check for.</param>
        /// <returns> True if the tag is found in the list of tags.</returns>
        public virtual bool HasTag(T tag)
        {
            return _tagList.Contains(tag);
        }

        
        /// <summary> The RemoveTag function removes a tag from the list of tags.</summary>
        /// <param name="tag"> The tag to be removed.</param>
        /// <returns> A boolean value.</returns>
        public virtual void RemoveTag(T tag)
        {
            _tagList.Remove(tag);
        }

        
        /// <summary> The RemoveTags function removes all tags from the tag list that are passed in as parameters.</summary>
        /// <param name="tags"> The tags to be removed from the tag list.</param>
        /// <returns> A list of type t</returns>
        public virtual void RemoveTags(params T[] tags)
        {
            _tagList.RemoveAll(t => tags.Contains(t));
        }
    }
}

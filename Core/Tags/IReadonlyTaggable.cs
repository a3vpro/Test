﻿//----------------------------------------------------------------------------
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
namespace VisionNet.Core.Tags
{
    public interface IReadonlyTaggable<T>
    {
        bool HasTag(T tag);
        bool HasAllTags(params T[] tags);
        bool HasAnyTag(params T[] tags);        
    }
}

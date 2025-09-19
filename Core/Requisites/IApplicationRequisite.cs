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
using Microsoft.Extensions.Options;
using VisionNet.Core.Abstractions;

namespace VisionNet.Core.Requisites
{
    public interface IApplicationRequisite : ICheckeable<InvalidCheckResult>, ICleanable, IConfigureOptions<ApplicationRequisitesOptions>
    {
        RequisiteType Type { get; }
    }
}

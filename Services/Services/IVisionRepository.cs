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
using VisionNet.Core.Patterns;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    public interface IVisionRepository : IReadOnlyRepository<List<IVisionFunction>, string>, IExceptionObservable
    {
        /// <summary>
        /// Obtiene una máquina de estados del tipo especificado por su identificador.
        /// </summary>
        /// <typeparam name="T">El tipo de máquina de estados a obtener.</typeparam>
        /// <param name="id">El identificador de la máquina de estados.</param>
        /// <returns>La máquina de estados solicitada.</returns>
        T Get<T>(string id) where T : List<IVisionFunction>, new();
    }
}

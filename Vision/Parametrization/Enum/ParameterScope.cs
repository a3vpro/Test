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
namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Enumerado de los distintos ámbitos de la ejecución de una función de vision
    /// </summary>
    public enum ParameterScope
    {
        /// <summary>
        /// Fase de inicialización (carga de patrones, establecimiento de áreas...)
        /// </summary>
        Initialization = 1,
        /// <summary>
        /// Fase de ejecución (tratamiento de la imagen, búsqueda de formas...)
        /// </summary>
        Execution = 2,
        /// <summary>
        /// Fase de finalización (liberación de memoria...)
        /// </summary>
        Finalization = 3
    }
}

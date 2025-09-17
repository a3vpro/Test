using System.Collections.Generic;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Patterns;

namespace VisionNet.Core
{
    public interface IRuntimeConfigurableService
    {
        /// <summary>
        /// Refresca los parámetros de runtime de los objetos gestionados en masa.
        /// Para cada entrada en el diccionario, se usa la key para obtener el objeto mediante Get 
        /// y se invoca RefreshRuntimeParameters con el options correspondiente.
        /// </summary>
        /// <param name="updatedOptionsDictionary">
        /// Diccionario que asocia un identificador (por ejemplo, Index o Name) con su instancia de IRuntimeOptions personalizada.
        /// </param>
        void RefreshAllRuntimeParameters(Dictionary<string, IRuntimeOptions> updatedOptionsDictionary);
    }
}

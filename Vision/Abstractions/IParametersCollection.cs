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
using System.Collections.Generic;
using VisionNet.Core.SafeObjects;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    public interface IParametersCollection: IReadonlyParametersCollection, IWriteParametersCollection
    {
        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        IParameter GetModificable(string id);

        /// <summary>
        /// Used to get a IList that is used to retrieve entities from entire table.
        /// </summary>
        /// <returns>Ilist to be used to select entities</returns>
        IList<IParameter> GetAllModificable();
    }
}

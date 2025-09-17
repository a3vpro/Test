//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 28-08-2021
// Description      : v1.0.0
//
// Copyright        : (C)  2021 by Sothis. All rights reserved.       
//----------------------------------------------------------------------------

namespace VisionNet.Core.Patterns
{
    public abstract class ModelBase<DM> : ObservableObject
    {
        #region Constructor

        protected ModelBase(DM domainObject)
        {
            DomainObject = domainObject;
        }

        #endregion

        #region Properties

        public DM DomainObject { get; set; }

        #endregion
    }
}

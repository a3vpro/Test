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
    /// <summary>
    /// Provides a base implementation for model objects that wrap a domain object and
    /// expose change notification capabilities through <see cref="ObservableObject"/>.
    /// </summary>
    /// <typeparam name="DM">Type of the domain object represented by the model.</typeparam>
    public abstract class ModelBase<DM> : ObservableObject
    {
        #region Constructor

        protected ModelBase(DM domainObject)
        {
            DomainObject = domainObject;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the domain object instance associated with the model.
        /// </summary>
        public DM DomainObject { get; set; }

        #endregion
    }
}

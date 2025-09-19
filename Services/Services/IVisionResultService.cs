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
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    public interface IVisionResultService
    {
        /// <summary>
        /// NNotify thah a new product is ready to be inspected
        /// </summary>
        /// <param name="systemSource">Identification of the location of the system (conveyor, cell, factory...).</param>
        /// <param name="pieceIndex">Unique identification of the piece or product.</param>
        /// <param name="creationTime">Start time of execution.</param>
        /// <param name="parameters">List of all the features of the product (It can be used to modify the behaviour of the execution).</param>
        void Init(ProductFeatures productFeatures);

        /// <summary>
        /// Post a new product vision result
        /// </summary>
        /// <param name="productResult">Product vision result to post</param>
        /// <param name="dateTime">Inspection time</param>
        void Post(ProductResult productResult, DateTime dateTime = default);

        /// <summary>
        /// Post a new inspection vision result (part of the product vision result)
        /// </summary>
        /// <param name="value">Product vision result to post</param>
        /// <param name="dateTime">Inspection time</param>
        void PostInspection(InspectionResult value, DateTime dateTime = default);

        /// <summary>
        /// Event raised when a new product is ready to be inspected
        /// </summary>
        event EventHandler<VisionProductFeaturesEventArgs> NewProductIsReadyToInspect;

        /// <summary>
        /// Event raised when a vision result is post
        /// </summary>
        event EventHandler<VisionResultEventArgs> NewProductResult;

        /// <summary>
        /// Event raised when a inspection vision result is post
        /// </summary>
        event EventHandler<VisionInspectionEventArgs> NewInspection;
    }
}

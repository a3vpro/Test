using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VisionNet.Image;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    public class ResultComposer
    {
        private ConcurrentDictionary<long, ProductResult> _procesingProducts = new ConcurrentDictionary<long, ProductResult>();
        private object _lockProductResult = new object();
        private TimeSpan _maxDuration = TimeSpan.FromMinutes(5);

        public ResultComposer(TimeSpan maxDuration)
        {
            _maxDuration = maxDuration;
        }

        /// <summary>
        /// Notify thah a new product is ready to be inspected
        /// </summary>
        /// <param name="systemSource">Identification of the location of the system (conveyor, cell, factory...).</param>
        /// <param name="pieceIndex">PLC identification of the piece or product.</param>
        /// <param name="internalIndex">Unique identification of the piece or product.</param>
        /// <param name="creationTime">Start time of execution.</param>
        /// <param name="parameters">List of all the features of the product (It can be used to modify the behaviour of the execution).</param>
        public ProductFeatures Init(string systemSource, long pieceIndex, long internalIndex, DateTime creationTime, params NamedValue[] parameters)
        {
            ProductFeatures tmpProductFeatures = null;

            lock (_lockProductResult)
            {
                var found = _procesingProducts.ContainsKey(pieceIndex);
                if (!found)
                {
                    // Creación del nuevo ProductResult
                    var productResult = ProductResultFactory.CreateNew(systemSource, pieceIndex, internalIndex, creationTime, parameters);
                    _procesingProducts.TryAdd(pieceIndex, productResult);
                    tmpProductFeatures = productResult.Features;
                }
                else
                {
                    // Si ya existe se devuelven sus características
                    found = _procesingProducts.TryGetValue(pieceIndex, out var productResult);
                    if (found)
                        tmpProductFeatures = productResult.Features;
                }
            }

            return tmpProductFeatures;
        }

        public void Finish(long pieceIndex)
        {
            TryFinish(pieceIndex);
        }

        public bool TryFinish(long pieceIndex)
        {
            return TryUpdate(pieceIndex, p =>
            {
                if (p.Status == ProductProcessStatus.Processing)
                {
                    p.CalculateResult();
                    p.Status = ProductProcessStatus.Finished;
                }
                return p;
            });
        }        

        public List<ImageResult> AddImages(long pieceIndex, IReadonlyParametersCollection outputParameters, List<NamedValue> inspectionStep = null)
        {
            var success = TryAddImages(pieceIndex, outputParameters, out var imagesResult, inspectionStep);

            //if (!success || imagesResult == null)
            if (!success)
                throw new InvalidOperationException($"Impossible to add a images to the result {pieceIndex}");

            return imagesResult;
        }

        public bool TryAddImages(long pieceIndex, IReadonlyParametersCollection outputParameters, out List<ImageResult> imagesResult, List<NamedValue> inspectionStep = null)
        {
            List<ImageResult> tmpImageResult = null;

            var result = TryUpdate(pieceIndex, p =>
            {
                if (p.Status == ProductProcessStatus.Processing)
                {
                    List<string> lstImagesToInclude = new List<string>();
                    foreach (IReadonlyParameter param in outputParameters.GetAll())
                    {
                        if (param.DataType == BasicTypeCode.Image && param.SaveToResult)
                        {
                            lstImagesToInclude.Add(param.Name);
                        }
                    }
                    string[] includeFilter = lstImagesToInclude.ToArray();
                    tmpImageResult = p.AddImages(outputParameters, inspectionStep, includeFilter);
                }

                return p;
            });

            imagesResult = tmpImageResult;
            return result && tmpImageResult != null;
        }

        public List<ImageResult> AddImages(long pieceIndex, IImageCollection sourceImages, List<NamedValue> inspectionStep = null)
        {
            var success = TryAddImages(pieceIndex, sourceImages, out var imagesResult, inspectionStep);

            if (!success || imagesResult == null)
                throw new InvalidOperationException($"Impossible to add a images to the result {pieceIndex}");

            return imagesResult;
        }

        public bool TryAddImages(long pieceIndex, IImageCollection sourceImages, out List<ImageResult> imagesResult, List<NamedValue> inspectionStep = null)
        {
            List<ImageResult> tmpImageResult = null;

            var result = TryUpdate(pieceIndex, p =>
            {
                if (p.Status == ProductProcessStatus.Processing)
                    tmpImageResult = p.AddImages(sourceImages, inspectionStep);
                return p;
            });

            imagesResult = tmpImageResult;
            return result && tmpImageResult != null;
        }

        public InspectionResult AddInspection(long pieceIndex, IOutputParametersCollection outputParameters, bool includeInResult, bool includeInStats = true, bool isSegmentationInspection = false, List<NamedValue> inspectionStep = null)
        {
            var success = TryAddInspection(pieceIndex, outputParameters, out var inspectionResult, includeInResult, includeInStats, isSegmentationInspection, inspectionStep);

            if (!success || inspectionResult == null)
                throw new InvalidOperationException($"Impossible to add a inspection to the result {pieceIndex}");

            return inspectionResult;
        }

        public bool TryAddInspection(long pieceIndex, IOutputParametersCollection outputParameters, out InspectionResult inspectionResult, bool includeInResult, bool includeInStats = true, bool isSegmentationInspection = false, List<NamedValue> inspectionStep = null)
        {
            InspectionResult tmpInspectionResult = null;

            var result = TryUpdate(pieceIndex, p =>
            {
                if (p.Status == ProductProcessStatus.Processing)
                    tmpInspectionResult = p.AddInspection(outputParameters, includeInResult, includeInStats, isSegmentationInspection, inspectionStep);
                return p;
            });

            inspectionResult = tmpInspectionResult;
            return result && tmpInspectionResult != null;
        }

        public bool TryAddInspection(long pieceIndex, InspectionResult inspectionResult)
        {
            var result = TryUpdate(pieceIndex, p =>
            {
                if (p.Status == ProductProcessStatus.Processing)
                {
                    p.Inspections.Add(inspectionResult);
                }

                var success = TryAddImages(pieceIndex, inspectionResult.OutputParameters, out var imagesResult, inspectionResult.Step);
                return p;
            });

            return result;
        }
        
        public void AddMeasurable(long pieceIndex, IReadonlyParameter parameter, ref InspectionResult inspectionResult)
        {
            var success = TryAddMeasurable(pieceIndex, parameter, ref inspectionResult);

            if (!success)
                throw new InvalidOperationException($"Impossible to add a parameter to the inspection result {pieceIndex}");
        }       
        
        public bool TryAddMeasurable(long pieceIndex, IReadonlyParameter parameter, ref InspectionResult inspectionResult)
        {
            InspectionResult tmpInspectionResult = inspectionResult;

            var result = TryUpdate(pieceIndex, p =>
            {
                if (p.Status == ProductProcessStatus.Processing && p.Inspections.Contains(tmpInspectionResult))
                {
                    tmpInspectionResult.AddMeasurable(parameter);
                }
                return p;
            });

            inspectionResult = tmpInspectionResult;
            return result && tmpInspectionResult != null;
        }         

        public bool TryExtract(long pieceIndex, out ProductResult productResult)
        {
            var success = false;
            productResult = null;

            lock (_lockProductResult)
            {
                success = _procesingProducts.ContainsKey(pieceIndex);
                if (!success)
                    return false;

                success = _procesingProducts.TryGetValue(pieceIndex, out productResult)
                    && productResult.Status == ProductProcessStatus.Finished;
                if (!success)
                    return false;

                success = _procesingProducts.TryRemove(pieceIndex, out productResult);
            }

            return success;
        }

        public bool TryGetPieceFeatures(long pieceIndex, out List<NamedValue> features)
        {
            var success = false;
            features = new List<NamedValue>();

            lock (_lockProductResult)
            {
                success = _procesingProducts.ContainsKey(pieceIndex);
                if (!success)
                    return false;

                success = _procesingProducts.TryGetValue(pieceIndex, out var productResult)
                    && productResult.Status == ProductProcessStatus.Processing;
                if (!success)
                    return false;
                features = productResult.Features.Parameters;
            }

            return success;
        }

        public bool Exists(long pieceIndex)
        {
            return _procesingProducts.ContainsKey(pieceIndex);
        }

        public bool TryUpdate(long pieceIndex, Func<ProductResult, ProductResult> modifyFunction)
        {
            var success = false;

            lock (_lockProductResult)
            {
                success = _procesingProducts.ContainsKey(pieceIndex);
                if (!success)
                    return false;

                success = _procesingProducts.TryGetValue(pieceIndex, out var productResult)
                    && productResult.Status == ProductProcessStatus.Processing;
                if (!success)
                    return false;

                productResult = modifyFunction(productResult);

                _procesingProducts.AddOrUpdate(pieceIndex, productResult, (i, p) => p);
            }

            return success;
        }

        public int RemoveObsolete()
        {
            var removed = 0;

            lock (_lockProductResult)
            {
                var now = DateTime.Now;

                var toRemoveListIndex = _procesingProducts.Values
                    .Where(p => p.Info.DateTime < now - _maxDuration)
                    .Select(p => p.Features.ExternalIndex); // Índice del PLC

                if (toRemoveListIndex.Count() == 0)
                    return 0;

                foreach (var toRemoveIndex in toRemoveListIndex)
                {
                    var success = _procesingProducts.TryRemove(toRemoveIndex, out var productResult);
                    if (success)
                        removed++;
                }
            }

            return removed;
        }
    }
}

using System;
using VisionNet.Core.Patterns;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    public class ProductResultFactory : Factory<ProductResult>
    {
        protected override ProductResult NewInstance()
        {
            return new ProductResult();
        }

        public ProductResult Create(string systemSource, long pieceIndex, long internalIndex, DateTime creationTime, params NamedValue[] productFeatures)
        {
            var features = new ProductFeatures
            {
                ExternalIndex = pieceIndex, // PLC Index
                Source = systemSource,
                DateTime = creationTime,
            };
            foreach (var feature in productFeatures)
                features.Parameters.Add(feature);

            var productResult = new ProductResult
            {
                //Index = pieceIndex,
                Status = ProductProcessStatus.Processing,
                Features = features
            };
            productResult.Info.Source = systemSource;
            productResult.Info.InternalIndex = internalIndex; // Unique Index
            productResult.Info.DateTime = DateTime.Now; // Provisional. Should be recalculated at the end of the process (CalculateResult)

            return productResult;
        }

        public static ProductResult CreateNew(string systemSource, long pieceIndex, long internalIndex, DateTime creationTime, params NamedValue[] parameters)
        {
            return new ProductResultFactory().Create(systemSource, pieceIndex, internalIndex, creationTime, parameters);
        }
    }
}

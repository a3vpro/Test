using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisionNet.Core.Patterns;

namespace VisionNet.Vision.Core
{
    public class PopulateInspectionsTransformer : ITransformer<ProductResult>
    {
        public void Transform(ref ProductResult productResult)
        {
            var result = new List<InspectionResult>();
            // Retrieve all inspections (standard + segmentation) that have been processed
            var allPopulatedInspections = productResult.Inspections
                .SelectMany(x => x.Populate());

            int count = 0;
            foreach (var inspection in allPopulatedInspections)
            {
                inspection.Order = count++;
                result.Add(inspection);
            }

            productResult.Inspections = result;
        }
    }
}

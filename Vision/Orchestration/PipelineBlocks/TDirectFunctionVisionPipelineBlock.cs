using System;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    public abstract class TDirectFunctionVisionPipelineBlock : TVisionPipelineBlock
    {
        protected IVisionFunction _visionFunction;

        public TDirectFunctionVisionPipelineBlock(
            ResultComposer resultComposer, 
            IVisionFunction visionFunction, 
            Func<string, string, string> performanceStringFunction,
            int boundedCapacity, 
            int maxDegreeOfParallelism
            ) : base(resultComposer, performanceStringFunction, boundedCapacity, maxDegreeOfParallelism)
        {
            _visionFunction = visionFunction;
        }

        #region Execution

        protected override IVisionFunction _getVisionFunction(string functionIndex)
        {
            return _visionFunction;
        }

        #endregion Execution
    }
}

using System;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    public abstract class AcDirectFunctionVisionPipelineBlock : AcVisionPipelineBlock
    {
        protected IVisionFunction _visionFunction;

        public IVisionFunction VisionFunction
        {
            get => _visionFunction;
            set => _visionFunction = value;
        }

        public AcDirectFunctionVisionPipelineBlock(
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

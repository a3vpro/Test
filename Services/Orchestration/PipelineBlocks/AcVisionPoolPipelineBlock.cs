using System;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.Services
{
    public abstract class AcVisionPoolPipelineBlock : AcVisionPipelineBlock
    {
        protected IVisionPool _visionPool;

        public AcVisionPoolPipelineBlock(
            ResultComposer resultComposer, 
            IVisionPool visionPool, 
            Func<string, string, string> performanceStringFunction,
            int boundedCapacity, 
            int maxDegreeOfParallelism
            ) : base(resultComposer, performanceStringFunction, boundedCapacity, maxDegreeOfParallelism)
        {
            _visionPool = visionPool;
        }

        #region Execution

        protected override IVisionFunction _getVisionFunction(string functionIndex)
        {
            return _visionPool.GetFromPool(functionIndex);
        }

        #endregion Execution
    }
}

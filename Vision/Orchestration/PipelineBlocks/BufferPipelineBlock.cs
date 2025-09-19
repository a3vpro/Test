using System.Threading.Tasks.Dataflow;

namespace VisionNet.Vision.Core
{
    public abstract class BufferPipelineBlock : PipelineBlock
    {
        internal BufferBlock<VisionMessage> _dataflowBlock;

        public override bool IsBlockCompleted
        {
            get
            {
                return !IsFunctionProcessing
                    && _dataflowBlock.Count == 0;
            }
        }

        public override int InputCount
        {
            get
            {
                return _dataflowBlock.Count;
            }
        }

        public BufferPipelineBlock(int boundedCapacity, int maxDegreeOfParallelism)
        {
            _dataflowBlock = new BufferBlock<VisionMessage>(
                new ExecutionDataflowBlockOptions()
                {
                    BoundedCapacity = boundedCapacity,
                    MaxDegreeOfParallelism = maxDegreeOfParallelism
                });

            SourceBlock = _dataflowBlock;
            TargetBlock = _dataflowBlock;
        }
    }
}

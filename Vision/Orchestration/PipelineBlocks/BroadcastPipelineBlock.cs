using System.Threading.Tasks.Dataflow;

namespace VisionNet.Vision.Core
{
    public abstract class BroadcastPipelineBlock : PipelineBlock
    {
        internal BroadcastBlock<VisionMessage> _dataflowBlock;

        public override bool IsBlockCompleted
        {
            get
            {
                return !IsFunctionProcessing;
            }
        }

        public BroadcastPipelineBlock(int boundedCapacity, int maxDegreeOfParallelism)
        {
            _dataflowBlock = new BroadcastBlock<VisionMessage>(
                vm => vm,
                new ExecutionDataflowBlockOptions()
                {
                    BoundedCapacity = boundedCapacity,
                    MaxDegreeOfParallelism = maxDegreeOfParallelism,
                });

            SourceBlock = _dataflowBlock;
            TargetBlock = _dataflowBlock;
        }
    }
}

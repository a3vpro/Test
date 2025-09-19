using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using VisionNet.Core.Events;

namespace VisionNet.Vision.Core
{
    public abstract class AcVisionPipelineBlock : VisionPipelineBlock
    {
        protected ActionBlock<VisionMessage> _dataflowBlock;

        public override int InputCount
        {
            get
            {
                return _dataflowBlock.InputCount;
            }
        }

        public override bool IsBlockCompleted
        {
            get
            {
                return !IsFunctionProcessing
                    && _dataflowBlock.InputCount == 0;
            }
        }

        public AcVisionPipelineBlock(
            ResultComposer resultComposer, 
            Func<string, string, string> performanceStringFunction, 
            int boundedCapacity, 
            int maxDegreeOfParallelism
            ) : base(resultComposer, performanceStringFunction)
        {
            _dataflowBlock = new ActionBlock<VisionMessage>(
                vm => _executeBlockFunction(vm),
                new ExecutionDataflowBlockOptions()
                {
                    BoundedCapacity = boundedCapacity,
                    MaxDegreeOfParallelism = maxDegreeOfParallelism,
                });

            TargetBlock = _dataflowBlock;
        }

        #region Execution

        protected virtual void _executeVisionFunction(VisionMessage vm)
        {
            // Execute Vision Function
            InspectionResult inspectionResult = _executeInspection(_functionName, vm);

            // Return de vision message
            _ = _resultComposer.TryAddInspection(vm.Index, inspectionResult);
            RaiseNewInspection(this, new EventArgs<InspectionResult>(inspectionResult));
        }

        protected virtual void _executeBlockFunction(VisionMessage vm)
        {
            Interlocked.Increment(ref _processCount);

            Console.WriteLine(_performanceStringFunction(GetFunctionName(), "Enter"));

            _executeVisionFunction(vm);

            Console.WriteLine(_performanceStringFunction(GetFunctionName(), "Exit"));

            Interlocked.Decrement(ref _processCount); // ALV: Siempre antes de la llamada a RaiseNewFinishedProcess dado que es bloque terminador de pipeline

            RaiseNewFinishedProcess(this, new EventArgs<long>(vm.Index));
        }

        #endregion Execution
    }
}

using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using VisionNet.Core.Events;

namespace VisionNet.Vision.Core
{
    public abstract class TVisionPipelineBlock : VisionPipelineBlock
    {
        protected TransformBlock<VisionMessage, VisionMessage> _dataflowBlock;

        public override int InputCount
        {
            get
            {
                return _dataflowBlock.InputCount;
            }
        }

        public override int OutputCount
        {
            get
            {
                return _dataflowBlock.OutputCount;
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

        public TVisionPipelineBlock(
            ResultComposer resultComposer, 
            Func<string, string, string> performanceStringFunction, 
            int boundedCapacity, 
            int maxDegreeOfParallelism
            ) : base(resultComposer, performanceStringFunction)
        {
            _dataflowBlock = new TransformBlock<VisionMessage, VisionMessage>(
                vm => _executeBlockFunction(vm),
                new ExecutionDataflowBlockOptions()
                {
                    BoundedCapacity = boundedCapacity,
                    MaxDegreeOfParallelism = maxDegreeOfParallelism,
                });

            SourceBlock = _dataflowBlock;
            TargetBlock = _dataflowBlock;
        }

        #region Execution

        protected virtual VisionMessage _executeVisionFunction(VisionMessage vm)
        {
            // Execute Vision Function
            InspectionResult inspectionResult = _executeInspection(_functionName, vm);

            // Return de vision message
            _ = _resultComposer.TryAddInspection(vm.Index, inspectionResult);
            RaiseNewInspection(this, new EventArgs<InspectionResult>(inspectionResult));

            // Always compose the output vision message
            var resultMessage = new VisionMessage(vm, _functionName, inspectionResult.OutputParameters);

            return resultMessage;
        }

        protected virtual VisionMessage _executeBlockFunction(VisionMessage vm)
        {
            Interlocked.Increment(ref _processCount);

            Console.WriteLine(_performanceStringFunction(GetFunctionName(), "Enter"));

            VisionMessage vmResult = _executeVisionFunction(vm);

            Console.WriteLine(_performanceStringFunction(GetFunctionName(), "Exit"));

            RaiseNewFinishedProcess(this, new EventArgs<long>(vm.Index));

            Interlocked.Decrement(ref _processCount); // ALV: Siempre despues de la llamada a RaiseNewFinishedProcess dado que nunca es bloque terminador de pipeline

            return vmResult;
        }

        #endregion Execution
    }
}

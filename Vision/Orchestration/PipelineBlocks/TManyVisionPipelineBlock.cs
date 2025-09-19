using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using VisionNet.Core.Events;

namespace VisionNet.Vision.Core
{
    public abstract class TManyVisionPipelineBlock : VisionPipelineBlock
    {
        protected TransformManyBlock<VisionMessage, VisionMessage> _dataflowBlock;

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

        public TManyVisionPipelineBlock(
            ResultComposer resultComposer, 
            Func<string, string, string> performanceStringFunction, 
            int boundedCapacity, 
            int maxDegreeOfParallelism
            ) : base(resultComposer, performanceStringFunction)
        {
            _dataflowBlock = new TransformManyBlock<VisionMessage, VisionMessage>(
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

        protected abstract List<VisionMessage> _executeVisionFunction(VisionMessage vm);

        protected virtual List<VisionMessage> _executeBlockFunction(VisionMessage vm)
        {
            Interlocked.Increment(ref _processCount);
            
            Console.WriteLine(_performanceStringFunction(GetFunctionName(), "Enter"));

            List<VisionMessage> vmResult = _executeVisionFunction(vm);

            Console.WriteLine(_performanceStringFunction(GetFunctionName(), "Exit"));

            //Se cambia el orden según si el pipeline va a seguir ejecutando o no, por el momento en el que se hace checkCompleted 
            if (vmResult.Count > 0)
            {
                RaiseNewFinishedProcess(this, new EventArgs<long>(vm.Index));

                Interlocked.Decrement(ref _processCount); // ALV: Siempre despues de la llamada a RaiseNewFinishedProcess dado que nunca es bloque terminador de pipeline
            }
            else
            {
                Interlocked.Decrement(ref _processCount); // ALV: Siempre antes de la llamada a RaiseNewFinishedProcess dado que siempre es bloque terminador de pipeline

                RaiseNewFinishedProcess(this, new EventArgs<long>(vm.Index));
            }

            return vmResult;
        }

        #endregion Execution
    }
}

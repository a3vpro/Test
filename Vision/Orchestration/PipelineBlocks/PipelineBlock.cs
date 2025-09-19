using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using VisionNet.Core.Events;
using VisionNet.Core.Exceptions;
using VisionNet.Core.Patterns;

namespace VisionNet.Vision.Core
{
    public abstract class PipelineBlock : IExceptionObservable
    {
        protected string _functionName;

        protected readonly PipelineStatusTransition _pipelineStatusTransition = new PipelineStatusTransition();

        public abstract bool IsBlockCompleted { get; }        

        public bool IsFunctionProcessing => ProcessCount > 0;

        public virtual int InputCount { get; } = 0;

        public virtual int OutputCount { get; } = 0;

        protected int _processCount = 0;
        public int ProcessCount => _processCount;

        public ISourceBlock<VisionMessage> SourceBlock { get; protected set; }

        public ITargetBlock<VisionMessage> TargetBlock { get; protected set; }

        public VisionPipelineStatus Status { get; protected set; } = VisionPipelineStatus.Initial;

        public PipelineBlock()
        {
            _functionName = GetFunctionName();
        }

        public abstract string GetFunctionName();

        public bool TrySetStatus(VisionPipelineStatus newStatus)
        {
            bool success = false;
            if (_pipelineStatusTransition.IsValidTransition(Status, newStatus))
            {
                Status = newStatus;
                success = true;
            }

            return success;
        }

        #region Events Region

        protected void RaiseNewInspection(object sender, EventArgs<InspectionResult> eventArgs)
        {
            try
            {
                NewInspection?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole();
            }
        }
        public event EventHandler<EventArgs<InspectionResult>> NewInspection;

        protected void RaiseNewFinishedProcess(object sender, EventArgs<long> eventArgs)
        {
            try
            {
                NewFinishedProcess?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole();
            }
        }
        public event EventHandler<EventArgs<long>> NewFinishedProcess;

        #endregion Events Region

        #region IExceptionObservable

        protected void RaiseExceptionNotification(object sender, ErrorEventArgs eventArgs)
        {
            try
            {
                ExceptionRaised?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(RaiseExceptionNotification));
            }
        }
        /// <summary>
        /// <summary> <inheritdoc/> </summary>
        /// </summary>
        public event EventHandler<ErrorEventArgs> ExceptionRaised;

        #endregion
    }
}

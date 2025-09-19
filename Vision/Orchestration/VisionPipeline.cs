using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;
using VisionNet.Core.Events;
using VisionNet.Core.Exceptions;
using VisionNet.Core.Patterns;
using VisionNet.Core.States;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    public abstract class VisionPipeline : IExceptionObservable, IPoolable
    {
        protected Dictionary<string, PipelineBlock> _dctPipelineBlocks = new Dictionary<string, PipelineBlock>();

        protected readonly PipelineStatusTransition _pipelineStatusTransition = new PipelineStatusTransition();

        #region Status
        public VisionPipelineStatus Status { get; protected set; } = VisionPipelineStatus.Initial;

        public bool IsInspectionCompleted
        {
            get
            {
                bool isInspectionCompleted = true;
                foreach (PipelineBlock pipelineBlock in _dctPipelineBlocks.Values)
                {
                    isInspectionCompleted &= pipelineBlock.IsBlockCompleted;
                }

                return isInspectionCompleted;
            }
        }

        public int TotalElements
        {
            get
            {
                return _dctPipelineBlocks.Values.Sum(block => block.InputCount + block.OutputCount);
            }
        }

        #endregion Status

        public VisionPipeline()
        {

        }

        #region Product Cycle

        /// <summary>
        /// Start the inspection cycle
        /// </summary>
        /// <param name="parameters">Modifiers of the inspection (recipe)</param>
        public void StartProductCicle()
        {
            _ = _trySetStatus(VisionPipelineStatus.Opened);
        }

        /// <summary>
        /// Add a message to the execution pipeline
        /// </summary>
        /// <param name="systemSource">Identification of the location of the system (conveyor, cell, factory...)</param>
        /// <param name="pieceIndex">Unique identification of the piece or product</param>
        /// <param name="inspectionStep">If the inspection of the piece or procuct has multiple steps, it identifies the step</param>
        /// <param name="sourceImages">List of the images to process</param>
        public abstract void EnqueueToExecution(string systemSource, long pieceIndex, List<NamedValue> inspectionStep, IImageCollection sourceImages);

        // Ends the inspection cycle
        public void EndProductCicle()
        {
            _ = _trySetStatus(VisionPipelineStatus.Closed);
        }

        public bool IsFinished(/*long pieceIndex*/)
        {
            //_checkCompleted(pieceIndex);
            return Status == VisionPipelineStatus.Completed;
        }

        #endregion Product Cycle

        #region PipelineBlocks

        protected virtual void _setPipelineBlocks()
        {
            throw new NotImplementedException("You have to create a personalized _setPipelineBlocks for the pipeline, it might require input parameters, like IVisionRepository");
        }

        protected abstract void _linkPipelineBlocks();

        #endregion PipelineBlocks

        #region Events Region
        protected void RaiseNewInspection(object sender, EventArgs<InspectionResult> eventArgs)
        {
            try
            {
                NewInspection?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                //RaiseExceptionNotification(this, new ErrorEventArgs(ex)); // Agregar la interfaz IExceptionObservable
                ex.LogToConsole();
            }
        }
        public event EventHandler<EventArgs<InspectionResult>> NewInspection;

        protected void RaiseNewResult(object sender, EventArgs<ProductResult> eventArgs)
        {
            try
            {
                NewResult?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                //RaiseExceptionNotification(this, new ErrorEventArgs(ex)); // Agregar la interfaz IExceptionObservable
                ex.LogToConsole();
            }
        }
        public event EventHandler<EventArgs<ProductResult>> NewResult;

        public void RaiseFreedPipeline(object sender, EventArgs<long> eventArgs)
        {
            try
            {
                FreedPipeline?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                //RaiseExceptionNotification(this, new ErrorEventArgs(ex)); // Agregar la interfaz IExceptionObservable
                ex.LogToConsole();
            }
        }
        public event EventHandler<EventArgs<long>> FreedPipeline;
        #endregion

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

        #region Private methods

        protected virtual bool _trySetStatus(VisionPipelineStatus newStatus)
        {
            bool success = false;
            if (_pipelineStatusTransition.IsValidTransition(Status, newStatus))
            {
                Status = newStatus;
                success = true;
            }

            return success;
        }

        protected abstract string _performaceString(string callerName, string actionDescription);

        protected abstract void _checkCompleted(long pieceIndex);

        #endregion

        #region IPoolable

        public bool IsOccupied { get; protected set; } = false;

        public void Hold()
        {
            IsOccupied = true;
        }

        public void Release(long pieceIndex)
        {
            IsOccupied = false;
            RaiseFreedPipeline(this, new EventArgs<long>(pieceIndex));
        }

        public void Release() { }

        #endregion IPoolable
    }
}

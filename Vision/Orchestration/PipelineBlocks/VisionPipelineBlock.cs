using System;
using System.Collections.Generic;
using System.IO;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Represents an abstract base class for a vision pipeline block, providing the structure and mechanisms 
    /// for executing vision functions and handling inspection results.
    /// </summary>
    public abstract class VisionPipelineBlock : PipelineBlock
    {
        /// <summary>
        /// The result composer used to aggregate or process results within the pipeline block.
        /// </summary>
        protected ResultComposer _resultComposer;

        /// <summary>
        /// A function delegate to generate a performance string based on input parameters.
        /// </summary>
        protected Func<string, string, string> _performanceStringFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionPipelineBlock"/> class with the specified result composer and performance string function.
        /// </summary>
        /// <param name="resultComposer">The result composer for processing inspection results.</param>
        /// <param name="performanceStringFunction">The function to create performance strings.</param>
        public VisionPipelineBlock(ResultComposer resultComposer, Func<string, string, string> performanceStringFunction) : base()
        {
            _resultComposer = resultComposer;
            _performanceStringFunction = performanceStringFunction;
        }

        #region Execution

        /// <summary>
        /// Retrieves the vision function associated with the specified function index.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="functionIndex">The index of the function to retrieve.</param>
        /// <returns>An instance of <see cref="IVisionFunction"/> corresponding to the function index.</returns>
        protected abstract IVisionFunction _getVisionFunction(string functionIndex);

        /// <summary>
        /// Executes an inspection using the specified function index and vision message.
        /// </summary>
        /// <param name="functionIndex">The index of the function to execute.</param>
        /// <param name="vm">The vision message containing the required data for inspection.</param>
        /// <returns>An <see cref="InspectionResult"/> representing the outcome of the inspection.</returns>
        protected virtual InspectionResult _executeInspection(string functionIndex, VisionMessage vm)
        {
            List<NamedValue> step = new List<NamedValue>(vm.Step);

            IVisionFunction visionFunction = _getVisionFunction(functionIndex);
            InspectionResult inspectionResult = visionFunction.NewEmptyInspectionResult(false, true, false, false, false, string.Empty, step);

            if (Status == VisionPipelineStatus.Opened || Status == VisionPipelineStatus.Closed)
            {
                try
                {
                    // Execute
                    IInputParametersCollection inputParameters = visionFunction.GetInputParameters(vm);
                    inspectionResult = visionFunction.Execute(inputParameters, step);
                }
                catch (Exception ex)
                {
                    // Return empty result in case of error
                    inspectionResult = visionFunction.NewEmptyInspectionResult(false, true, false, false, false, string.Empty, step);
                    RaiseExceptionNotification(this, new ErrorEventArgs(ex));
                }
            }
            else
            {
                string strError = $"Not valid status {Status}";
                inspectionResult.Error = strError;
                ((OutputParametersCollection)inspectionResult.OutputParameters).Error = strError;
            }

            visionFunction.Release(); // No, si esta clase no tiene la responsabilidad de hacer el hold, ¿como va a tener la responsabilidad de hacer el release?

            return inspectionResult;
        }

        #endregion Execution
    }
}

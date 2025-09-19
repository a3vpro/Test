using System.Collections.Generic;
using System.Linq;
using VisionNet.Core.Enums;
using VisionNet.Core.Patterns;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Clase conversora entre el tipo IOutputParametersCollection a InspectionResult
    /// </summary>
    public class InspectionResultAdapter: IAdapter<IOutputParametersCollection, InspectionResult>
    {
        public InspectionResult Convert(IOutputParametersCollection value)
        {
            var result = new InspectionResult();
            Convert(value, ref result);
            return result;
        }

        public InspectionResult Convert(IOutputParametersCollection value, bool includeInResult = true, bool includeInStats = true, bool isSegmentationInspection = false, List<NamedValue> step = null)
        {
            var result = new InspectionResult();
            Convert(value, ref result, includeInResult, includeInStats, isSegmentationInspection, step);
            return result;
        }

        public void Convert(IOutputParametersCollection value, ref InspectionResult result, bool includeInResult = true, bool includeInStats = true, bool isSegmentationInspection = false, List<NamedValue> step = null)
        {
            result = new InspectionResult();

            // 1. Adición de Inspecciones
            result.Name = value.ParentName;
            result.Enabled = value.Enabled;
            result.PrevResult = value.PrevResult;
            result.Result = value.Result;
            result.Success = value.Success;
            result.Error = value.Error;
            result.Warning = value.Warning;
            result.IncludeInResult = includeInResult; 
            result.IncludeInStats = includeInStats;
            result.IsSegmentationInspection = isSegmentationInspection;
            result.ProcessTime = value.ProcessTime;
            result.Measurables = new List<NamedValue>();
            result.OutputParameters = value;

            // 2. Adición de medibles
            var outputParameters = value.ToMeasurablesParameters
                .Where(v => v.DataType.IsNotIn(
                    BasicTypeCode.Image, 
                    //BasicTypeCode.Object, // Comentadop orque se almacenar como medible todo tipo de valores para que le llegue a la interfaz visual.
                    // TODO Esto no se debe guardar en BBDD ¿ni en JSON?
                    BasicTypeCode.Graphic,
                    BasicTypeCode.NotSupported));
            foreach (var outputParameter in outputParameters)
            {
                var measureable = new MeasurableAdapter().Convert(outputParameter);
                result.Measurables.Add(measureable);
            }

            // 3. Adición de paso de la inspección
            result.Step = step ?? new List<NamedValue>();
        }
    }
}

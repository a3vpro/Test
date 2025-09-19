using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisionNet.Core.Patterns;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    public class VisionMessageJoiner
    {
        public VisionMessage Join(params VisionMessage[] visionMessages)
        {
            return Join(-1, visionMessages);
        }

        public VisionMessage Join(long index, params VisionMessage[] visionMessages)
        {
            VisionMessage result = null;
            var restrictedGuid = index >= 0;

            visionMessages.Requires(nameof(visionMessages)).IsNotNull(); // visionMessages no debe ser nulo

            var selectedMesssages = restrictedGuid
                ? visionMessages.Where(vms => vms.PieceIndex == index)
                : visionMessages;

            selectedMesssages.Requires(nameof(visionMessages)).IsNotEmpty(); // visionMessages no debe ser vacio

            var usedIndex = visionMessages.First().PieceIndex;
            var startTime = visionMessages.First().AcquisitionMoment;
            var endTime = visionMessages.First().FinalizationMoment;

            // Unión de las imagenes de origen (si hay duplicados se toma la primera)
            var sourceImages = new ImageCollection();
            foreach (var si in selectedMesssages.SelectMany(vms => vms.SourceImages))
                if (!sourceImages.ContainsKey(si.Key))
                    sourceImages[si.Key] = si.Value;

            // Unión de los resultados de las inspecciones (si hay duplicados se toma la primera)
            var outputParameters = new Dictionary<string, OutputParametersCollection>();
            foreach (var op in selectedMesssages.SelectMany(vms => vms.PrevResults))
                if (!outputParameters.ContainsKey(op.Key))
                    outputParameters[op.Key] = op.Value;

            // Composición del resultado final
            result = new VisionMessage()
            {
                PieceIndex = visionMessages[0].PieceIndex,
                SourceImages = sourceImages,
                PrevResults = outputParameters,
                AcquisitionMoment = startTime,
                FinalizationMoment = endTime,
            };

            return result;
        }
    }
}

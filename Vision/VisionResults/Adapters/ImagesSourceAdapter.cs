using System;
using System.Collections.Generic;
using VisionNet.Core.Patterns;
using VisionNet.Image;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Clase conversora entre el tipo IImageCollection a List<ImageResult>
    /// </summary>
    public class ImagesSourceAdapter : IAdapter<IImageCollection, List<ImageResult>>
    {
        public List<ImageResult> Convert(IImageCollection value)
        {
            var result = new List<ImageResult>();
            Convert(value, ref result);
            return result;
        }

        public List<ImageResult> Convert(IImageCollection value, List<NamedValue> inspectionStep)
        {
            var result = new List<ImageResult>();
            Convert(value, ref result, inspectionStep);
            return result;
        }

        public void Convert(IImageCollection value, ref List<ImageResult> result, List<NamedValue> inspectionStep = null)
        {
            result = new List<ImageResult>();

            // Adición de las imagenes de entrada al ProductResult
            foreach (var imageSource in value)
            {
                var image = imageSource.Value as IImage;
                // Se añaden las imagenes creadas en la adquisición de las cámaras
                // Se añade el step del key (en este caso el orden de la lista)
                var inspectionStepExtended = new List<NamedValue>(inspectionStep ?? new List<NamedValue>());
                inspectionStepExtended.Add(
                new NamedValue
                {
                    Name = "Order",
                    Type = BasicTypeCode.String,
                    Value = imageSource.Key.ToString(),
                });

                var imageResult = new ImageResult
                {
                    Name = image.DeviceSource, // Compongo el nombre a base del nombre del dispositivo y los steps
                    Image = image,
                    FileName = string.Empty, // La ruta se compondrá al finalizar toda la inspección y enviarla a guardar
                    SourceType = ImageResultSourceType.AcquisitionDevice, // Se trata de una imagen resultado de la adquisición de un dispositivo
                    SourceName = image.DeviceSource, // Al tratarse de una imagen de entrada, el origen es la cámara
                    AcquisitionMoment = image.CreationTime, // El momento de adquisición de la imagen
                    InspectionStep = inspectionStepExtended
                };

                result.Add(imageResult);
            }
        }
    }
}

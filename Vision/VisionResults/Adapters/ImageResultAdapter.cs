using System;
using System.Collections.Generic;
using VisionNet.Core.Patterns;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Clase conversora entre el tipo IOutputParametersCollection a List<ImageResult>
    /// </summary>
    public class ImageResultAdapter: IAdapter<IReadonlyParameter, ImageResult>
    {
        public ImageResult Convert(IReadonlyParameter value)
        {
            var result = new ImageResult();
            Convert(value, ref result);
            return result;
        }

        public ImageResult Convert(IReadonlyParameter value, List<NamedValue> inspectionStep)
        {
            var result = new ImageResult();
            Convert(value, ref result, inspectionStep);
            return result;
        }

        public void Convert(IReadonlyParameter value, ref ImageResult result, List<NamedValue> inspectionStep = null)
        {
            result = new ImageResult();

            var isImageParameter = value.DataType == BasicTypeCode.Image
                    && (!value.IsArray && value.Value is IImage);

            if (isImageParameter)
            {
                var image = value.Value as IImage;
                var imageResult = new ImageResult
                {
                    Name = value.Index, // El nombre es el parámetro y su step
                    Image = image,
                    FileName = string.Empty, // La ruta se compondrá al finalizar toda la inspección y enviarla a guardar
                    SourceType = ImageResultSourceType.InspectionResult, // Se trata de una imagen resultado de una inspección
                    SourceName = image.DeviceSource, // El origen es la cámara
                    AcquisitionMoment = image.CreationTime, // El momento de creación de la imagen
                    InspectionStep = inspectionStep ?? new List<NamedValue>()
                };
                result = imageResult;
            }
        }
    }
}

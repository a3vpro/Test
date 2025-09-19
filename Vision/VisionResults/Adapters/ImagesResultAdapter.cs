using System;
using System.Collections.Generic;
using System.Linq;
using VisionNet.Core.Collections;
using VisionNet.Core.Patterns;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Clase conversora entre el tipo IOutputParametersCollection a List<ImageResult>
    /// </summary>
    public class ImagesResultAdapter: IAdapter<IReadonlyParametersCollection, List<ImageResult>>
    {
        private bool _shouldFilter;
        private List<string> _includeFilter = new List<string>();

        public ImagesResultAdapter(bool shouldFilter = false, params string[] includeFilter)
        {
            _shouldFilter = shouldFilter;
            _includeFilter = includeFilter.ToList();
        }

        public List<ImageResult> Convert(IReadonlyParametersCollection value)
        {
            var result = new List<ImageResult>();
            Convert(value, ref result);
            return result;
        }

        public List<ImageResult> Convert(IReadonlyParametersCollection value, List<NamedValue> inspectionStep)
        {
            var result = new List<ImageResult>();
            Convert(value, ref result, inspectionStep);
            return result;
        }

        public void Convert(IReadonlyParametersCollection value, ref List<ImageResult> result, List<NamedValue> inspectionStep = null)
        {
            result = new List<ImageResult>();

            // 1. Se añaden las imágenes creadas como resultado de las inspecciones
            var generatedImages = value.GetAll()
                .Where(p =>
                    p.DataType == BasicTypeCode.Image
                    && ((!p.IsArray && p.Value is IImage)
                    || (p.IsArray && (p.Value is List<IImage>))));

            if (_shouldFilter)
                generatedImages = generatedImages
                    .Where(p =>
                    _includeFilter.Contains(p.Name));

            foreach (var generatedImage in generatedImages)
            {
                if (!generatedImage.IsArray)
                {
                    var image = generatedImage.Value as IImage;
                    var imageResult = new ImageResult
                    {
                        Name = generatedImage.Index, // El nombre es el parámetro y su step
                        Image = image,
                        FileName = string.Empty, // La ruta se compondrá al finalizar toda la inspección y enviarla a guardar
                        SourceType = ImageResultSourceType.InspectionResult, // Se trata de una imagen resultado de una inspección
                        SourceName = image.DeviceSource, // El origen es la inspección y el índice
                        AcquisitionMoment = image.CreationTime, // El momento de creación de la imagen
                        InspectionStep = inspectionStep ?? new List<NamedValue>()
                    };
                    result.Add(imageResult);
                }
                else
                {
                    var imageList = generatedImage.Value as List<IImage>;
                    //var i = 0;
                    foreach (var image in imageList)
                    {
                        var imageResult = new ImageResult
                        {
                            Name = generatedImage.Index, // El nombre es el parámetro y su step
                            Image = image,
                            FileName = string.Empty, // La ruta se compondrá al finalizar toda la inspección y enviarla a guardar
                            SourceType = ImageResultSourceType.InspectionResult, // Se trata de una imagen resultado de una inspección
                            //SourceName = $"{image.DeviceSource}_{i}", // El origen es la inspección y el índice
                            SourceName = image.DeviceSource, // El origen es la inspección y el índice
                            AcquisitionMoment = image.CreationTime, // El momento de creación de la imagen
                            InspectionStep = inspectionStep ?? new List<NamedValue>()
                        };
                        result.Add(imageResult);
                        //i++;
                    }
                }
            }
        }
    }
}

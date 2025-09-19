using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisionNet.Core.Patterns;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    public class VisionMessageAdapter : IAdapter<VisionMessage, ProductResult>
    {
        public ProductResult Convert(VisionMessage value)
        {
            // 1. Creación de resultado de imágenes
            var imagesResult = new List<ImageResult>();
            // Se añaden las imagenes originales
            foreach (var imageSource in value.SourceImages)
            {
                var imageResult = new ImageResult
                {
                    Name = imageSource.Key.ToString(),
                    Image = imageSource.Value,
                    FileName = String.Empty, // Falta componer la ruta. DECIDIR SI AÑADIR LA RUTA AHORA Y CUANDO SE GUARDE !!
                    Source = "Camera", // No está bien resuelto. NO TENGO EL ORIGEN !!
                    AcquisitionMoment = DateTime.Now, // No está bien resuelto. NO TENGO EL MOMENTO DE ADQUISICIÓN !!
                };
                imagesResult.Add(imageResult);
            }
            // Se añaden las imagenes creadas como resultado de las inspecciones
            var generatedImagenes = value.PrevResults
                .SelectMany(r => r.Value.Parameters.Values)
                .Where(p => p.DataType == BasicTypeCode.Image);

            foreach (var generatedImage in generatedImagenes)
            {
                var imageResult = new ImageResult
                {
                    Name = generatedImage.Name,
                    Image = generatedImage.Value as IImage,
                    FileName = String.Empty, // Falta componer la ruta. DECIDIR SI AÑADIR LA RUTA AHORA Y CUANDO SE GUARDE !!
                    Source = value.SystemSource,
                    AcquisitionMoment = value.AcquisitionMoment,
                };
                imagesResult.Add(imageResult);
            }

            // 2. Creación de Inspecciones
            var inspectionsResult = new List<InspectionResult>();
            foreach (var inspection in value.PrevResults)
            {
                var inspectionResult = new InspectionResult();
                inspectionResult.Name = inspection.Key;
                inspectionResult.Enabled = inspection.Value.Enabled;
                inspectionResult.PrevResult = inspection.Value.PrevResult;
                inspectionResult.Result = inspection.Value.Result;
                inspectionResult.Success = inspection.Value.Success;
                inspectionResult.Error = inspection.Value.Error;
                inspectionResult.ProcessTime = inspection.Value.ProcessTime;
                inspectionResult.Measurables = new List<ValueResult>();
                var outputParameters = inspection.Value.NotReservedParameters.Values
                    .Where(v => v.DataType != BasicTypeCode.Image);

                foreach (var outputParameter in outputParameters)
                {
                    var measureable = new ValueResult();
                    measureable.Name = outputParameter.Name;
                    measureable.Value = outputParameter.Value;
                    measureable.Type = outputParameter.DataType;
                    inspectionResult.Measurables.Add(measureable);
                }

                inspectionsResult.Add(inspectionResult);
            }

            // 3. Creación de información de resulado
            var productInfoResult = new ProductInfoResult();
            productInfoResult.Index = value.PieceIndex;
            productInfoResult.Source = value.SystemSource;
            productInfoResult.Result = inspectionsResult.All(ir => ir.Result);
            productInfoResult.Enabled = inspectionsResult.All(ir => ir.Enabled);
            productInfoResult.Success = inspectionsResult.All(ir => ir.Success);
            productInfoResult.Error = String.Join(". ", inspectionsResult.Select(ir => ir.Error));
            productInfoResult.ProcessTime = (value.FinalizationMoment - value.AcquisitionMoment).TotalMilliseconds;
            productInfoResult.DateTime = value.AcquisitionMoment;
            productInfoResult.ByPass = false; // Falta por rellenar
            productInfoResult.Forced = false; // Falta por rellenar

            // Se crea el resulado del producto
            var productResult = new ProductResult
            {
                Images = imagesResult,
                Inspections = inspectionsResult,
                Parameters = value.Parameters,
                Info = productInfoResult,
            };

            // Devolución del resultado
            return productResult;
        }
    }
}

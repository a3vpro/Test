using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisionNet.Core.Types;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
	public static class ProductResultExtensions
	{        
		public static ImageResult AddImage(this ProductResult productResult, IReadonlyParameter parameter, List<NamedValue> inspectionStep = null)
		{
			// Se añaden las imagenes creadas como resultado de las inspecciones
			var imageResult = new ImageResultAdapter().Convert(parameter, inspectionStep);
			if (imageResult != null)
			productResult.Images.Add(imageResult);
			return imageResult;
		}        

		public static List<ImageResult> AddImages(this ProductResult productResult, IReadonlyParametersCollection outputParameters, List<NamedValue> inspectionStep = null, params string[] includeFilter)
		{
			// Se añaden las imagenes creadas como resultado de las inspecciones
			var imagesResult = new ImagesResultAdapter(true, includeFilter).Convert(outputParameters, inspectionStep);
			if (imagesResult != null && imagesResult.Count > 0)
			productResult.Images.AddRange(imagesResult);
			return imagesResult;
		}

		public static List<ImageResult> AddImages(this ProductResult productResult, IImageCollection sourceImages, List<NamedValue> inspectionStep = null)
		{
			// Se añaden las imagenes creadas en la adquisición de las cámaras
			var imagesResult = new ImagesSourceAdapter().Convert(sourceImages, inspectionStep);
			if (imagesResult != null && imagesResult.Count > 0)
			productResult.Images.AddRange(imagesResult);
			return imagesResult;
		}

		public static InspectionResult AddInspection(this ProductResult productResult, IOutputParametersCollection outputParameters, bool includeInResult = true, bool includeInStats = true, bool isSegmentationInspection = false, List<NamedValue> inspectionStep = null)
		{
			// Creación de Inspecciones
			var inspectionResult = new InspectionResultAdapter().Convert(outputParameters, includeInResult, includeInStats, isSegmentationInspection, inspectionStep);
			productResult.Inspections.Add(inspectionResult);
			return inspectionResult;
		}
		public static void AddMeasurable(this InspectionResult inspectionResult, IReadonlyParameter parameter)
		{
			var measurable = new MeasurableAdapter().Convert(parameter);
			inspectionResult.Measurables.Add(measurable);
		}

		public static void CalculateResult(this ProductResult productResult)
		{
			var inspections = productResult.Inspections;

			bool inspectionsResult = inspections.Where(i => i.IncludeInResult).All(i => i.Result);
			//bool inspectionsResult = inspections.All(i => i.Result);
			bool inspectionsSuccess = inspections.All(i => i.Success);
			bool inspectionsEnabled = inspections.Any(i => i.Enabled);
			bool inspectionsWarning = inspections.Any(i => i.Warning);

			var productResultInfo = new ProductInfoResult();
			productResultInfo.ByPass = false; // TODO
			productResultInfo.Forced = false; // TODO
			productResultInfo.Enabled = inspectionsEnabled;
			productResultInfo.Source = productResult.Features.Source;
			productResultInfo.DateTime = DateTime.Now;
			productResultInfo.Warning = inspectionsWarning;
			productResultInfo.Result = inspectionsResult;
			productResultInfo.Success = inspectionsSuccess;
			productResultInfo.InternalIndex = productResult.Info.InternalIndex;
            productResultInfo.Id = Guid.NewGuid();
            productResultInfo.ProcessTime = (productResultInfo.DateTime - productResult.Features.DateTime).TotalMilliseconds;

			// Construir conjunto de mensajes de error para el productResult
			var sb = new StringBuilder();
			foreach (var inspection in inspections)
            {
                if (!string.IsNullOrEmpty(inspection.Error))
				{
					_ = sb.Append(inspection.Error)
					  .Append(Environment.NewLine);
				}
            }

			productResultInfo.Error = sb.ToString();

			productResult.Info = productResultInfo;
		}
	}
}

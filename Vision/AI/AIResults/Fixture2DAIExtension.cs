using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisionNet.Vision.AI;
using VisionNet.Vision.Core;

namespace VisionNet.Vision.AI
{
    public static class Fixture2DAIExtension
    {
        /// <summary>
        /// Mapea una instancia de SingleInstSegmentationInferenceResult a través de la transformación de Fixture2D.
        /// Se mapean las propiedades BoundingBox, Center, Position y Polygon.
        /// </summary>
        /// <param name="fixture">La instancia de Fixture2D que realizará el mapeo.</param>
        /// <param name="instance">La instancia de SingleInstSegmentationInferenceResult a mapear.</param>
        /// <returns>Una nueva instancia de SingleInstSegmentationInferenceResult con los valores transformados.</returns>
        public static SingleInstSegmentationInferenceResult MapSingleInstance(this Fixture2D fixture, SingleInstSegmentationInferenceResult instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return new SingleInstSegmentationInferenceResult
            {
                ClassIndex = instance.ClassIndex,
                ClassName = instance.ClassName,
                ExtraInfo = instance.ExtraInfo,
                Score = instance.Score,
                Center = fixture.MapPoint(instance.Center),
                BoundingBox = fixture.MapBoundingBox(instance.BoundingBox),
                Polygon = fixture.MapPolygon(instance.Polygon),
                Position = fixture.MapCoordinateSystem(instance.Position),
                Mask = instance.Mask,
                SortCriteriaParameters = instance.SortCriteriaParameters,
                SelectionCriteriaParameters = instance.SelectionCriteriaParameters,
                Result = instance.Result,
                Selected = instance.Selected,
            };
        }
    }
}

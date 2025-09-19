using VisionNet.Vision.Core;
using System.Drawing;
using Newtonsoft.Json;
using System.Collections.Generic;
using VisionNet.Image;

namespace VisionNet.Vision.AI
{
    public class SingleInstSegmentationInferenceResult
    {
        public int ClassIndex { get; set; }
        public string ClassName { get; set; }
        public InstanceExtraInfo ExtraInfo { get; set; }
        public double Score { get; set; }
        public PointF Center { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public Polygon Polygon { get; set; }

        public CoodinateSystem Position { get; set; }
        [JsonIgnore]
        public Bitmap Mask { get; set; }

        public string Source { get; set; }

        #region Sorting
        public SortCriteria SortCriteriaParameters = SortCriteria.Default;

        private double? _cachedWeight = null;
        [JsonIgnore]
        public double Weight
        {
            get
            {
                if (!_cachedWeight.HasValue)
                    CalcWeight(SortCriteriaParameters);
                return _cachedWeight.Value;
            }
        }

        public double CalcWeight(SortCriteria sortCriteriaParameters)
        {
            SortCriteriaParameters = sortCriteriaParameters;
            _cachedWeight = sortCriteriaParameters.CalcWeight(Center.X, Center.Y, BoundingBox.Diagonal, Score, Polygon.Area);
            return _cachedWeight.Value;
        }
        #endregion

        public SelectionCriteria SelectionCriteriaParameters { get; set; }

        private SelectionStatus? _cachedSelected = SelectionStatus.Unselected;
        [JsonIgnore]
        public SelectionStatus Selected
        {
            get
            {
                return _cachedSelected.Value;
            }
            set
            {
                _cachedSelected = value;
            }
        }

        [JsonIgnore]
        public bool Result { get; set; }// A rellenar por la inspección

        public SelectionStatus FitSelectionCriteria(SelectionCriteria filterCriteriaParameters)
        {
            SelectionCriteriaParameters = filterCriteriaParameters;
            _cachedSelected = filterCriteriaParameters.FitCriteria(BoundingBox.Diagonal, Score, Polygon.Area, Weight);
            return _cachedSelected.Value;
        }

        public void CalculateSelected()
        {
            FitSelectionCriteria(SelectionCriteriaParameters);
        }


        public void SetSelectionstatus(SelectionStatus newSelectedValue)
        {
            _cachedSelected = newSelectedValue;
        }

        /// <summary>
        /// Reescala el objeto actual modificando en sitio los valores transformados (Center, BoundingBox, Polygon y Position)
        /// según el factor de escala indicado.
        /// </summary>
        /// <param name="pixelSize">Factor de escala (por ejemplo, 2.0 para duplicar las dimensiones).</param>
        public void Rescale(double pixelSize)
        {
            // Recalcular el centro
            Center = new PointF((float)(Center.X * pixelSize), (float)(Center.Y * pixelSize));

            // Recalcular el BoundingBox si existe
            if (BoundingBox != null)
            {
                BoundingBox.X = (float)(BoundingBox.X * pixelSize);
                BoundingBox.Y = (float)(BoundingBox.Y * pixelSize);
                BoundingBox.Width = (float)(BoundingBox.Width * pixelSize);
                BoundingBox.Height = (float)(BoundingBox.Height * pixelSize);
                // El ángulo se mantiene sin cambios.
            }

            // Recalcular el Polygon si existe
            if (Polygon != null)
                Polygon = Polygon.Scale((float)pixelSize);

            //// Recalcular el Polygon si existe
            //if (Polygon != null)
            //{
            //    for (int i = 0; i < Polygon.Shape.Count; i++)
            //    {
            //        PointF pt = Polygon.Shape[i];
            //        Polygon.Shape[i] = new PointF((float)(pt.X * pixelSize), (float)(pt.Y * pixelSize));
            //    }
            //    // Se recalcula el área si el método CalcArea() lo requiere.
            //    Polygon.CalcArea();
            //}

            //// Recalcular los agujeros si existen
            //if (Holes != null && Holes.Count > 0)
            //{
            //    for (int h = 0; h < Holes.Count; h++)
            //    {
            //        for (int i = 0; i < Holes[h].Shape.Count; i++)
            //        {
            //            PointF pt = Holes[h].Shape[i];
            //            Holes[h].Shape[i] = new PointF((float)(pt.X * pixelSize), (float)(pt.Y * pixelSize));
            //        }
            //        // Se recalcula el área si el método CalcArea() lo requiere.
            //        Holes[h].CalcArea();
            //    }
            //}

            // Recalcular el sistema de coordenadas (Position) si existe
            if (Position != null)
            {
                Position.Origin = new PointF((float)(Position.Origin.X * pixelSize), (float)(Position.Origin.Y * pixelSize));
                // El ángulo se mantiene sin cambios.
            }
        }
    }
}

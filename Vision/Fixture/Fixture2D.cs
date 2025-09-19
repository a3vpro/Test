using System;
using System.Collections.Generic;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using VisionNet.Graphic;
using VisionNet.Image;

namespace VisionNet.Vision.Core
{
    public class Fixture2D : IFixture2D
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Angle { get; private set; }
        public double ScaleX { get; private set; }
        public double ScaleY { get; private set; }

        public Matrix<double> TransformMatrix { get; private set; }

        public Fixture2D() : this(0, 0, 0, 1, 1) { }

        public Fixture2D(double x, double y, double angle, double scaleX = 1.0, double scaleY = 1.0)
        {
            X = x;
            Y = y;
            Angle = angle;
            ScaleX = scaleX;
            ScaleY = scaleY;
            ComputeMatrix();
        }

        public Fixture2D(PointF translations, double angle, double scaleX = 1.0, double scaleY = 1.0)
            : this(translations.X, translations.Y, angle, scaleX, scaleY) { }

        private Fixture2D(Matrix<double> matrix)
        {
            if (matrix.RowCount != 3 || matrix.ColumnCount != 3)
                throw new ArgumentException("La matriz debe ser de 3x3.");

            TransformMatrix = matrix.Clone();

            X = TransformMatrix[0, 2];
            Y = TransformMatrix[1, 2];

            ScaleX = Math.Sqrt(Math.Pow(TransformMatrix[0, 0], 2) + Math.Pow(TransformMatrix[1, 0], 2));
            ScaleY = Math.Sqrt(Math.Pow(TransformMatrix[0, 1], 2) + Math.Pow(TransformMatrix[1, 1], 2));

            if (ScaleX > 1e-12)
                Angle = Math.Atan2(TransformMatrix[1, 0] / ScaleX, TransformMatrix[0, 0] / ScaleX);
            else
                Angle = 0;
        }

        private void ComputeMatrix()
        {
            double cos = Math.Cos(Angle);
            double sin = Math.Sin(Angle);

            TransformMatrix = DenseMatrix.OfArray(new double[,]
            {
                { cos * ScaleX, -sin * ScaleY, X },
                { sin * ScaleX,  cos * ScaleY, Y },
                { 0,             0,            1 }
            });
        }

        public void GetRotationTranslation(out double x, out double y, out double angle)
        {
            x = X;
            y = Y;
            angle = Angle;
        }

        public bool Identity(double tolerance = 1e-10)
        {
            return Math.Abs(X) < tolerance &&
                   Math.Abs(Y) < tolerance &&
                   (Math.Abs(Angle) < tolerance || Math.Abs(Angle - 2 * Math.PI) < tolerance) &&
                   Math.Abs(ScaleX - 1.0) < tolerance &&
                   Math.Abs(ScaleY - 1.0) < tolerance;
        }

        public IFixture2D Invert()
        {
            if (Math.Abs(TransformMatrix.Determinant()) < 1e-12)
                throw new InvalidOperationException("No se puede invertir la transformación: determinante cero.");

            var inv = TransformMatrix.Inverse();
            return new Fixture2D(inv);
        }

        public IFixture2D LinearTransform(double fromX, double fromY)
        {
            var lin = DenseMatrix.OfArray(new double[,]
            {
                { TransformMatrix[0, 0], TransformMatrix[0, 1], 0 },
                { TransformMatrix[1, 0], TransformMatrix[1, 1], 0 },
                { 0,                     0,                     1 }
            });
            return new Fixture2D(lin);
        }

        public double MapAngle(double angleIn)
        {
            double vx = Math.Cos(angleIn);
            double vy = Math.Sin(angleIn);

            MapVector(ref vx, ref vy);

            return Math.Atan2(vy, vx);
        }

        public void MapPoint(double x, double y, out double mappedX, out double mappedY)
        {
            var pt = DenseVector.OfArray(new double[] { x, y, 1 });
            var res = TransformMatrix * pt;

            mappedX = res[0];
            mappedY = res[1];
        }

        public PointF MapPoint(PointF inputPoint)
        {
            MapPoint(inputPoint.X, inputPoint.Y, out double mappedX, out double mappedY);
            return new PointF((float)mappedX, (float)mappedY);
        }

        public List<double> MapPoints(List<double> points)
        {
            if (points == null || points.Count % 2 != 0)
                throw new ArgumentException("La lista de puntos debe contener pares (x, y).");

            var lstMappedPoints = new List<double>(points.Count);
            for (int i = 0; i < points.Count; i += 2)
            {
                double x = points[i];
                double y = points[i + 1];
                MapPoint(x, y, out double mx, out double my);
                lstMappedPoints.Add(mx);
                lstMappedPoints.Add(my);
            }

            return lstMappedPoints;
        }

        public List<PointF> MapPoints(List<PointF> points)
        {
            if (points == null)
                throw new ArgumentException("La lista de puntos no debe ser null");

            var lstMappedPoints = new List<PointF>(points.Count);
            foreach (var pt in points)
            {
                lstMappedPoints.Add(MapPoint(pt));
            }

            return lstMappedPoints;
        }

        public void MapVector(ref double x, ref double y)
        {
            double newX = TransformMatrix[0, 0] * x + TransformMatrix[0, 1] * y;
            double newY = TransformMatrix[1, 0] * x + TransformMatrix[1, 1] * y;
            x = newX;
            y = newY;
        }

        public CoodinateSystem MapCoordinateSystem(CoodinateSystem cs)
        {
            if (cs == null)
                throw new ArgumentNullException(nameof(cs));

            MapPoint(cs.Origin.X, cs.Origin.Y, out double mappedX, out double mappedY);
            double mappedAngle = MapAngle(cs.Angle);

            return new CoodinateSystem
            {
                Origin = new PointF((float)mappedX, (float)mappedY),
                Angle = mappedAngle
            };
        }

        public BoundingBox MapBoundingBox(BoundingBox box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));

            MapPoint(box.X, box.Y, out double mappedX, out double mappedY);
            double mappedAngle = MapAngle(box.Angle);

            float newWidth = (float)(box.Width * ScaleX);
            float newHeight = (float)(box.Height * ScaleY);

            return new BoundingBox
            {
                X = (float)mappedX,
                Y = (float)mappedY,
                Width = newWidth,
                Height = newHeight,
                Angle = Convert.ToSingle(mappedAngle)
            };
        }

        public Polygon MapPolygon(Polygon polygon)
        {
            if (polygon == null)
                throw new ArgumentNullException(nameof(polygon));

            var mappedPoly = new Polygon();

            foreach (PointF pt in polygon.Shape)
            {
                MapPoint(pt.X, pt.Y, out double mappedX, out double mappedY);
                mappedPoly.Add(new PointF((float)mappedX, (float)mappedY));
            }

            foreach (var child in polygon.Children)
                mappedPoly.AddChildren(MapPolygon(child));

            return mappedPoly;
        }

        public void SetRotationTranslation(double x, double y, double angle)
        {
            X = x;
            Y = y;
            Angle = angle;
            ComputeMatrix();
        }

        public void MultiplyScaleAndTranslation(double factorX, double factorY)
        {
            ScaleX *= factorX;
            ScaleY *= factorY;
            X *= factorX;
            Y *= factorY;
            ComputeMatrix();
        }

        public void SetScale(double newScaleX, double newScaleY)
        {
            double factorX = newScaleX / ScaleX;
            double factorY = newScaleY / ScaleY;
            MultiplyScaleAndTranslation(factorX, factorY);
        }

        public override string ToString()
        {
            return $"Fixture2D: Traslación=({X}, {Y}), Ángulo={Angle} rad, Escala=({ScaleX}, {ScaleY})";
        }

        public static Fixture2D operator *(Fixture2D a, Fixture2D b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            var composedMatrix = a.TransformMatrix * b.TransformMatrix;
            return new Fixture2D(composedMatrix);
        }

        public static Fixture2D CreateFixtureFromRectangleAffine(IGraphicRectangleAffine rect, bool fromCenter)
        {
            // Traslación: centro del recorte
            double tx = rect.CenterX;
            double ty = rect.CenterY;

            // Rotación: ya viene en radianes
            double angle = rect.Rotation;

            // Escalado: tamaño del recorte
            double scaleX = 1.0;
            double scaleY = 1.0;

            // Construcción del Fixture2D
            var fixture = new Fixture2D(tx, ty, angle, scaleX, scaleY);

            // Queremos ahora trasladar el punto relativo al recorte (0,0 en esquina sup. izq.)
            // al sistema centrado en (0,0), así que creamos una fixture auxiliar que mueva el origen
            double offsetX = fromCenter ? 0 : - rect.SideXLength / 2.0;
            double offsetY = fromCenter ? 0 : -rect.SideYLength / 2.0;

            var offset = new Fixture2D(offsetX, offsetY, 0);
            return fixture * offset; // Combina ambas transformaciones
        }

        public static Fixture2D CreateFixtureFromRectangle(IGraphicRectangle rect, bool fromCenter)
        {
            // Traslación
            double tx = fromCenter ? rect.CenterX : rect.X;
            double ty = fromCenter ? rect.CenterY : rect.Y;

            // Rotación
            double angle = 0;

            // Escalado
            double scaleX = 1.0;
            double scaleY = 1.0;

            // Construcción del Fixture2D
            var fixture = new Fixture2D(tx, ty, angle, scaleX, scaleY);

            return fixture;
        }
    }
}

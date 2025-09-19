using System;
using System.Collections.Generic;
using System.Text;

namespace VisionNet.Vision.Core
{
    public interface IFixture2D
    {
        // Propiedades
        double X { get; }
        double Y { get; }
        double Angle { get; }
        double ScaleX { get; }
        double ScaleY { get; }

        /// <summary>
        /// Obtiene la traslación y el ángulo de rotación de la transformación.
        /// </summary>
        /// <param name="x">Traslación en X (salida).</param>
        /// <param name="y">Traslación en Y (salida).</param>
        /// <param name="angle">Ángulo de rotación en radianes (salida).</param>
        void GetRotationTranslation(out double x, out double y, out double angle);

        /// <summary>
        /// Indica si la transformación es la identidad, dentro de una tolerancia.
        /// </summary>
        /// <param name="tolerance">Tolerancia para la comparación.</param>
        /// <returns>True si es identidad; de lo contrario, false.</returns>
        bool Identity(double tolerance = 1e-10);

        /// <summary>
        /// Retorna una nueva transformación que es la inversa de la transformación actual.
        /// </summary>
        /// <returns>Una instancia de IFixture2D que representa la transformación inversa.</returns>
        IFixture2D Invert();

        /// <summary>
        /// Retorna la transformación lineal (o derivada/Jacobiano) evaluada en el punto dado.
        /// Como la transformación es afín, esta parte es constante.
        /// </summary>
        /// <param name="fromX">Coordenada X del punto (en el sistema 'from').</param>
        /// <param name="fromY">Coordenada Y del punto (en el sistema 'from').</param>
        /// <returns>Una instancia de IFixture2D que representa la parte lineal.</returns>
        IFixture2D LinearTransform(double fromX, double fromY);

        /// <summary>
        /// Mapea el ángulo dado (en radianes) a través de la parte lineal de la transformación.
        /// </summary>
        /// <param name="angleIn">Ángulo de entrada en radianes.</param>
        /// <returns>Ángulo mapeado en radianes.</returns>
        double MapAngle(double angleIn);

        /// <summary>
        /// Mapea un punto (x, y) a través de la transformación.
        /// </summary>
        /// <param name="x">Coordenada X del punto de entrada.</param>
        /// <param name="y">Coordenada Y del punto de entrada.</param>
        /// <param name="mappedX">Coordenada X mapeada (salida).</param>
        /// <param name="mappedY">Coordenada Y mapeada (salida).</param>
        void MapPoint(double x, double y, out double mappedX, out double mappedY);

        /// <summary>
        /// Mapea una lista de puntos a través de la transformación.
        /// Se espera que la lista contenga pares (x, y) consecutivos.
        /// </summary>
        /// <param name="points">Lista de puntos [x0, y0, x1, y1, ...].</param>
        /// <returns>Lista de puntos mapeados.</returns>
        List<double> MapPoints(List<double> points);

        /// <summary>
        /// Mapea un vector (x, y) a través de la parte lineal de la transformación.
        /// La traslación se ignora.
        /// </summary>
        /// <param name="x">Componente X del vector (entrada y salida por referencia).</param>
        /// <param name="y">Componente Y del vector (entrada y salida por referencia).</param>
        void MapVector(ref double x, ref double y);

        /// <summary>
        /// Actualiza la traslación y el ángulo de la transformación.
        /// La escala se mantiene sin cambios.
        /// </summary>
        /// <param name="x">Nueva traslación en X.</param>
        /// <param name="y">Nueva traslación en Y.</param>
        /// <param name="angle">Nuevo ángulo de rotación en radianes.</param>
        void SetRotationTranslation(double x, double y, double angle);
    }

}

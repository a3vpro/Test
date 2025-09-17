using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using NetTopologySuite.Geometries;
using VisionNet.Core.Dawing;

namespace VisionNet.Core.AI
{
    public class BuildAndExpandClustersPolygons
    {
        /// <summary>
        /// Procesa una lista de clusters, construyendo y expandiendo su polígono correspondiente.
        /// </summary>
        public List<Polygon> Execute(
            List<List<PointF>> clusters,
            double expansionDistance = 5.0) // píxeles
        {
            var polygons = new List<Polygon>();

            foreach (var cluster in clusters)
            {
                // Usa el método estático para construir y expandir el polígono
                var polygon = ExecuteSingle(cluster, expansionDistance);

                polygons.Add(polygon);
            }

            return polygons;
        }

        /// <summary>
        /// Construye y expande el polígono del cluster dado su lista de puntos.
        /// </summary>
        /// <param name="points">Lista de puntos (sin contexto) del cluster.</param>
        /// <param name="expansionDistance">Distancia en píxeles para expandir el polígono.</param>
        /// <returns>Polígono expandido que cubre el cluster.</returns>
        public Polygon ExecuteSingle(List<PointF> points, double expansionDistance = 5.0)
        {
            var factory = new GeometryFactory();

            if (points == null || points.Count == 0)
                return null;

            var coords = points.Select(p => new Coordinate(p.X, p.Y)).ToArray();
            var multipoint = factory.CreateMultiPointFromCoords(coords);
            var convexHull = multipoint.ConvexHull() as Polygon;
            var expanded = convexHull.Buffer(expansionDistance) as Polygon;

            return expanded;
        }
    }
}

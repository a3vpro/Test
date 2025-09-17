using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using NetTopologySuite.Geometries;
using VisionNet.Core.Dawing;

namespace VisionNet.Core.AI
{
    /// <summary>
    /// Provides helpers for converting unordered image-space clusters into convex polygons
    /// and expanding those polygons by a configurable buffer to guarantee full coverage of
    /// the cluster footprint.
    /// </summary>
    public class BuildAndExpandClustersPolygons
    {
        /// <summary>
        /// Generates a buffered polygon for every cluster in the collection by creating the
        /// cluster&apos;s convex hull and expanding it with the specified buffer distance.
        /// </summary>
        /// <param name="clusters">Collection of clusters where each cluster is a list of <see cref="PointF"/> instances in image coordinates. The outer list must not be <see langword="null"/>.</param>
        /// <param name="expansionDistance">Non-negative distance in pixels used as the buffer applied around each convex hull. Defaults to 5 pixels.</param>
        /// <returns>A list containing the buffered polygon for each input cluster. Entries can be <see langword="null"/> when the source cluster is <see langword="null"/> or empty.</returns>
        /// <exception cref="System.NullReferenceException">Thrown when <paramref name="clusters"/> is <see langword="null"/>.</exception>
        /// <exception cref="NetTopologySuite.Geometries.TopologyException">Propagated when NetTopologySuite fails to build a valid geometry for a cluster.</exception>
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
        /// Builds a convex hull for the provided cluster of points and returns an expanded
        /// polygon buffered by the supplied distance.
        /// </summary>
        /// <param name="points">Unordered cluster points expressed in image pixel coordinates. Can be <see langword="null"/> or empty to indicate the absence of a valid cluster.</param>
        /// <param name="expansionDistance">Non-negative distance in pixels used to buffer the resulting polygon. Defaults to 5 pixels.</param>
        /// <returns>The buffered polygon representing the cluster, or <see langword="null"/> when <paramref name="points"/> is <see langword="null"/> or empty.</returns>
        /// <exception cref="NetTopologySuite.Geometries.TopologyException">Propagated when the convex hull or buffer operation fails to produce a valid polygon.</exception>
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

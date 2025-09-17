using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VisionNet.Core.Dawing;
using VisionNet.Drawing;

namespace VisionNet.Core.AI
{
    /// <summary>
    /// Implements the K-Means clustering algorithm for 2D points.
    /// </summary>
    public class KMeansClusteringGeneric
    {
        private readonly KMeansClustering<object> _cluster = new KMeansClustering<object>();

        /// <summary>
        /// Number of clusters to form.
        /// </summary>
        public int ClusterCount
        {
            get => _cluster.ClusterCount;
            set => _cluster.ClusterCount = value;
        }

        /// <summary>
        /// Gets or sets the list of points to be clustered.
        /// </summary>
        public List<PointF> Points { get; set; } = new List<PointF>();

        /// <summary>
        /// Gets the list of clusters formed after executing the algorithm.
        /// Each cluster is a list of PointF.
        /// </summary>
        public List<List<PointF>> Clusters { get; private set; } = new List<List<PointF>>();

        /// <summary>
        /// Adds a point to the clustering algorithm.
        /// </summary>
        /// <param name="point">The point to add.</param>
        public void AddPoint(PointF point)
        {
            Points.Add(point);
        }

        /// <summary>
        /// Adds a point by coordinates to the clustering algorithm.
        /// </summary>
        public void AddPoint(float x, float y)
        {
            Points.Add(new PointF(x, y));
        }

        /// <summary>
        /// Clears all points and clusters.
        /// </summary>
        public void Clear()
        {
            Points.Clear();
            Clusters.Clear();
        }

        /// <summary>
        /// Executes the K-Means clustering algorithm with the current settings.
        /// </summary>
        public void Execute()
        {
            // Convert to context-wrapped points (context unused)
            var ptsWithContext = Points
                .Select(p => new PointFWithContext<object>(p, null))
                .ToList();

            // Delegate to generic implementation
            _cluster.Points = ptsWithContext;
            _cluster.Execute();

            // Convert results back to List<PointF>
            Clusters = _cluster.Clusters
                .Select(cluster => cluster.Select(c => c.Point).ToList())
                .ToList();
        }
    }
}

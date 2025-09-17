using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using VisionNet.Core.Dawing;
using VisionNet.Drawing;

namespace VisionNet.Core.AI
{
    /// <summary>
    /// Implements a clustering algorithm that groups points based on a provided Euclidean distance threshold.
    /// </summary>
    public class CompleteLinkageHierarchicalClusteringGeneric
    {
        private CompleteLinkageHierarchicalClustering<object> _cluster = new CompleteLinkageHierarchicalClustering<object>();

        /// <summary>
        /// Gets or sets the list of points to be clustered.
        /// </summary>
        public List<PointF> Points { get; set; } = new List<PointF>();

        /// <summary>
        /// Gets or sets the Euclidean distance threshold used to determine if two points should belong to the same cluster.
        /// </summary>
        public float DistanceThreshold { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the list of clusters formed after executing the clustering algorithm.
        /// </summary>
        public List<List<PointF>> Clusters { get; set; } = new List<List<PointF>>();

        /// <summary>
        /// Adds a point to the clustering algorithm.
        /// </summary>
        /// <param name="point">The point to add.</param>
        public void AddPoint(PointF point)
        {
            Points.Add(point);
        }

        /// <summary>
        /// Adds a point with the specified coordinates to the clustering algorithm.
        /// </summary>
        /// <param name="x">The X-coordinate of the point.</param>
        /// <param name="y">The Y-coordinate of the point.</param>
        public void AddPoint(float x, float y)
        {
            Points.Add(new PointF(x, y));
        }

        /// <summary>
        /// Clears all points and clusters from the algorithm.
        /// </summary>
        public void Clear()
        {
            Points.Clear();
            Clusters.Clear();
        }

        /// <summary>
        /// Executes the clustering algorithm using the current set of points and distance threshold.
        /// </summary>
        public void Execute()
        {
            // Get the inputs
            var points = Points;
            var distanceThreshold = DistanceThreshold;

            // Execution
            var clusters = Execute(points, distanceThreshold);

            // Set the outputs
            Clusters = Clusters;
        }

        /// <summary>
        /// Clusters a list of PointF objects such that the Euclidean distance between any two points in the same cluster is less than or equal to the specified threshold.
        /// </summary>
        /// <param name="points">The list of points to cluster.</param>
        /// <param name="distanceThreshold">The maximum allowed distance between any two points in the same cluster.</param>
        /// <returns>A list of clusters, each cluster is a list of PointF objects.</returns>
        public List<List<PointF>> Execute(List<PointF> points, double distanceThreshold)
        {
            // Convert points to PointFWithContext objects
            List<PointFWithContext<object>> pointsWithContext = points.Select(i => new PointFWithContext<object>(i, null)).ToList();

            // Execute the clustering algorithm
            List<List<PointFWithContext<object>>> clustersWithContext = _cluster.Execute(pointsWithContext, distanceThreshold);

            // Convert the clusters back to a list of PointF objects
            List<List<PointF>> clusters = clustersWithContext.Select(c => c.Select(p => p.Point).ToList()).ToList();

            return clusters;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using VisionNet.Core.Dawing;
using VisionNet.Drawing;

namespace VisionNet.Core.AI
{
    /// <summary>
    /// Implements a clustering algorithm that groups points based on a provided Euclidean distance threshold.
    /// </summary>
    /// <typeparam name="T">The type of context associated with each point.</typeparam>
    public class CompleteLinkageHierarchicalClustering<T>
    {
        /// <summary>
        /// Gets or sets the list of points to be clustered.
        /// </summary>
        public List<PointFWithContext<T>> Points { get; set; } = new List<PointFWithContext<T>>();

        /// <summary>
        /// Gets or sets the Euclidean distance threshold used to determine if two points should belong to the same cluster.
        /// </summary>
        public float DistanceThreshold { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the list of clusters formed after executing the clustering algorithm.
        /// </summary>
        public List<List<PointFWithContext<T>>> Clusters { get; set; } = new List<List<PointFWithContext<T>>>();

        /// <summary>
        /// Adds a point with context to the clustering algorithm.
        /// </summary>
        /// <param name="point">The point to add.</param>
        public void AddPoint(PointFWithContext<T> point)
        {
            Points.Add(point);
        }

        /// <summary>
        /// Adds a point and its associated context to the clustering algorithm.
        /// </summary>
        /// <param name="point">The point to add.</param>
        /// <param name="context">The context associated with the point.</param>
        public void AddPoint(PointF point, T context)
        {
            Points.Add(new PointFWithContext<T>(point, context));
        }

        /// <summary>
        /// Adds a point and its associated context to the clustering algorithm using the specified coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the point.</param>
        /// <param name="y">The Y-coordinate of the point.</param>
        /// <param name="context">The context associated with the point.</param>
        public void AddPoint(float x, float y, T context)
        {
            Points.Add(new PointFWithContext<T>(x, y, context));
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
        /// Clusters a list of PointFWithContext<T> objects such that the Euclidean distance between any two points in the same cluster is less than or equal to the specified threshold.
        /// </summary>
        /// <param name="points">The list of points with context to cluster.</param>
        /// <param name="distanceThreshold">The maximum allowed distance between any two points in the same cluster.</param>
        /// <returns>A list of clusters, each cluster is a list of PointFWithContext<T> objects.</returns>
        public List<List<PointFWithContext<T>>> Execute(List<PointFWithContext<T>> points, double distanceThreshold)
        {
            // List to hold the final clusters
            List<List<PointFWithContext<T>>> clusters = new List<List<PointFWithContext<T>>>();

            // Set to keep track of unassigned points
            List<PointFWithContext<T>> unassignedPoints = new List<PointFWithContext<T>>(points);

            while (unassignedPoints.Count > 0)
            {
                // Initialize a new cluster
                List<PointFWithContext<T>> cluster = new List<PointFWithContext<T>>();
                // Get the first unassigned point
                PointFWithContext<T> seedPoint = unassignedPoints[0];
                cluster.Add(seedPoint);
                unassignedPoints.Remove(seedPoint);

                // List to hold points to check
                List<PointFWithContext<T>> pointsToCheck = new List<PointFWithContext<T>> { seedPoint };

                while (pointsToCheck.Count > 0)
                {
                    PointFWithContext<T> currentPoint = pointsToCheck[0];
                    pointsToCheck.RemoveAt(0);

                    List<PointFWithContext<T>> pointsToAdd = new List<PointFWithContext<T>>();

                    // Iterate over unassigned points
                    foreach (PointFWithContext<T> point in unassignedPoints)
                    {
                        // Check if the distance between the current point and the unassigned point is less than or equal to the threshold
                        if (currentPoint.Point.EuclideanDistance(point.Point) <= distanceThreshold)
                        {
                            // Check if the point is within the threshold distance to all points in the cluster
                            bool withinThreshold = true;
                            foreach (PointFWithContext<T> clusterPoint in cluster)
                            {
                                if (clusterPoint.Point.EuclideanDistance(point.Point) > distanceThreshold)
                                {
                                    withinThreshold = false;
                                    break;
                                }
                            }

                            if (withinThreshold)
                            {
                                pointsToAdd.Add(point);
                            }
                        }
                    }

                    // Add the points that meet the criteria to the cluster and pointsToCheck list
                    foreach (PointFWithContext<T> point in pointsToAdd)
                    {
                        cluster.Add(point);
                        pointsToCheck.Add(point);
                        unassignedPoints.Remove(point);
                    }
                }

                // Add the formed cluster to the list of clusters
                clusters.Add(cluster);
            }

            return clusters;
        }
    }
}
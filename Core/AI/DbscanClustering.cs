using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisionNet.Core.Dawing;
using VisionNet.Drawing;

namespace VisionNet.Core.AI
{
    /// <summary>
    /// Provides a data set agnostic implementation of the DBSCAN clustering algorithm for <see cref="PointFWithContext{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of contextual metadata associated with each clustered point.</typeparam>
    public class DbscanClustering<T>
    {
        /// <summary>
        /// Gets or sets the collection of data points to be clustered, each paired with its contextual metadata.
        /// </summary>
        /// <value>A non-null list containing the input <see cref="PointFWithContext{T}"/> instances to analyze.</value>
        public List<PointFWithContext<T>> Points { get; set; } = new List<PointFWithContext<T>>();

        /// <summary>
        /// Gets or sets the maximum Euclidean distance between two points for one to be considered within the same neighborhood.
        /// </summary>
        /// <value>A non-negative radius, in the same units as the point coordinates, that defines the density reachability threshold. Defaults to 5.</value>
        public float Epsilon { get; set; } = 5.0f;

        /// <summary>
        /// Gets or sets the minimum number of neighboring points required for a point to be considered a core point.
        /// </summary>
        /// <value>An integer greater than or equal to 1 that determines the cluster density requirement. Defaults to 5.</value>
        public int MinPoints { get; set; } = 5;

        /// <summary>
        /// Gets the clusters discovered by the most recent execution of the algorithm.
        /// </summary>
        /// <value>A list of clusters, where each cluster contains the <see cref="PointFWithContext{T}"/> instances assigned to it. The list is empty until <see cref="Execute()"/> is called.</value>
        public List<List<PointFWithContext<T>>> Clusters { get; private set; } = new List<List<PointFWithContext<T>>>();

        /// <summary>
        /// Executes the DBSCAN clustering algorithm on the configured <see cref="Points"/> using the current <see cref="Epsilon"/> and <see cref="MinPoints"/> settings.
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown when <see cref="Points"/> is <see langword="null"/>.</exception>
        public void Execute()
        {
            int clusterId = 0;
            var visited = new HashSet<int>();
            var noise = new HashSet<int>();
            var labels = new int[Points.Count];
            for (int i = 0; i < labels.Length; i++) labels[i] = -1;

            for (int i = 0; i < Points.Count; i++)
            {
                if (visited.Contains(i)) continue;
                visited.Add(i);
                var neighbors = RegionQuery(i);

                if (neighbors.Count < MinPoints)
                {
                    noise.Add(i);
                }
                else
                {
                    ExpandCluster(i, neighbors, clusterId, visited, labels);
                    clusterId++;
                }
            }

            Clusters = Enumerable.Range(0, clusterId)
                .Select(id => Points.Where((p, idx) => labels[idx] == id).ToList())
                .ToList();
        }

        private void ExpandCluster(int pointIdx, List<int> neighbors, int clusterId, HashSet<int> visited, int[] labels)
        {
            labels[pointIdx] = clusterId;
            var seeds = new Queue<int>(neighbors);

            while (seeds.Count > 0)
            {
                int current = seeds.Dequeue();
                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    var currentNeighbors = RegionQuery(current);
                    if (currentNeighbors.Count >= MinPoints)
                    {
                        foreach (var n in currentNeighbors)
                        {
                            if (!seeds.Contains(n))
                                seeds.Enqueue(n);
                        }
                    }
                }
                if (labels[current] == -1)
                    labels[current] = clusterId;
            }
        }

        private List<int> RegionQuery(int pointIdx)
        {
            var neighbors = new List<int>();
            var p = Points[pointIdx].Point;
            for (int i = 0; i < Points.Count; i++)
            {
                if (i == pointIdx) continue;
                var dist = p.EuclideanDistance(Points[i].Point);
                if (dist <= Epsilon)
                    neighbors.Add(i);
            }
            return neighbors;
        }
    }
}

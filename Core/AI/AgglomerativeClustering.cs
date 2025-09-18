using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VisionNet.Core.Dawing;
using VisionNet.Drawing;

namespace VisionNet.Core.AI
{
    /// <summary>
    /// Represents a hierarchical agglomerative clustering engine that groups two-dimensional points with attached context data
    /// by iteratively merging the closest clusters until the requested number of clusters is reached.
    /// </summary>
    /// <typeparam name="T">Type of contextual information associated with each point.</typeparam>
    public class AgglomerativeClustering<T>
    {
        /// <summary>
        /// Gets or sets the collection of source points, where each entry defines a two-dimensional coordinate paired with optional
        /// contextual data that participates in the clustering process. The collection must contain at least <see cref="ClusterCount"/>
        /// elements to form that many clusters and must not contain <see langword="null"/> entries.
        /// </summary>
        public List<PointFWithContext<T>> Points { get; set; } = new List<PointFWithContext<T>>();

        /// <summary>
        /// Gets or sets the target number of clusters to produce. The value must be greater than zero and less than or equal to
        /// the number of available <see cref="Points"/> when <see cref="Execute"/> is invoked.
        /// </summary>
        public int ClusterCount { get; set; } = 3;

        /// <summary>
        /// Gets the resulting hierarchical clusters, where each nested list contains the points assigned to the same cluster after
        /// <see cref="Execute"/> completes. The property is populated only after clustering has been performed.
        /// </summary>
        public List<List<PointFWithContext<T>>> Clusters { get; private set; }

        /// <summary>
        /// Executes the agglomerative clustering process on the configured <see cref="Points"/> until exactly <see cref="ClusterCount"/>
        /// clusters remain by repeatedly merging the two closest clusters according to Euclidean distance.
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown when <see cref="Points"/> is <see langword="null"/> or contains <see langword="null"/>
        /// elements, preventing distance calculations.</exception>
        public void Execute()
        {
            int n = Points.Count;
            // Inicializa clusters individuales
            Clusters = Points.Select(p => new List<PointFWithContext<T>> { p }).ToList();

            // Matriz de distancias (solo superior, simétrica)
            var distances = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    distances[i, j] = Points[i].Point.EuclideanDistance(Points[j].Point);

            while (Clusters.Count > ClusterCount)
            {
                // Encuentra los clusters más cercanos
                double minDist = double.MaxValue;
                int idx1 = -1, idx2 = -1;

                for (int i = 0; i < Clusters.Count; i++)
                {
                    for (int j = i + 1; j < Clusters.Count; j++)
                    {
                        double dist = MinimumDistanceBetweenClusters(Clusters[i], Clusters[j]);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            idx1 = i;
                            idx2 = j;
                        }
                    }
                }

                // Une clusters idx1 e idx2
                Clusters[idx1].AddRange(Clusters[idx2]);
                Clusters.RemoveAt(idx2);
            }
        }

        private double MinimumDistanceBetweenClusters(List<PointFWithContext<T>> c1, List<PointFWithContext<T>> c2)
        {
            double minDist = double.MaxValue;
            foreach (var p1 in c1)
                foreach (var p2 in c2)
                {
                    double dist = p1.Point.EuclideanDistance(p2.Point);
                    if (dist < minDist)
                        minDist = dist;
                }
            return minDist;
        }
    }

}

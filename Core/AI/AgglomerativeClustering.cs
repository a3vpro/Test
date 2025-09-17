using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VisionNet.Core.Dawing;
using VisionNet.Drawing;

namespace VisionNet.Core.AI
{
    public class AgglomerativeClustering<T>
    {
        public List<PointFWithContext<T>> Points { get; set; } = new List<PointFWithContext<T>>();
        public int ClusterCount { get; set; } = 3;
        public List<List<PointFWithContext<T>>> Clusters { get; private set; }

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

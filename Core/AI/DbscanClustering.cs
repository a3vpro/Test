using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisionNet.Core.Dawing;
using VisionNet.Drawing;

namespace VisionNet.Core.AI
{
    public class DbscanClustering<T>
    {
        public List<PointFWithContext<T>> Points { get; set; } = new List<PointFWithContext<T>>();
        public float Epsilon { get; set; } = 5.0f;
        public int MinPoints { get; set; } = 5;
        public List<List<PointFWithContext<T>>> Clusters { get; private set; } = new List<List<PointFWithContext<T>>>();

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

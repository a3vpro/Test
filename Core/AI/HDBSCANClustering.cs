using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VisionNet.Core.Dawing;
using VisionNet.Drawing;

namespace VisionNet.Core.AI
{
    /// <summary>
    /// Provides a simplified HDBSCAN-based clustering routine that partitions the configured point collection
    /// into dense groups based on mutual reachability distances derived from the minimum cluster size.
    /// </summary>
    /// <typeparam name="T">Type of contextual information carried with each clustered point.</typeparam>
    public class HdbscanClustering<T>
    {
        /// <summary>
        /// Gets or sets the collection of data points, each paired with contextual metadata, that will be
        /// evaluated for clustering when <see cref="Execute"/> is invoked.
        /// </summary>
        /// <remarks>
        /// The list must be non-null and should contain at least <see cref="MinClusterSize"/> elements to form
        /// a cluster; otherwise, only smaller clusters or an empty result will be produced.
        /// </remarks>
        /// <exception cref="NullReferenceException">Thrown by <see cref="Execute"/> if this property is set to <c>null</c>.</exception>
        public List<PointFWithContext<T>> Points { get; set; } = new List<PointFWithContext<T>>();

        /// <summary>
        /// Gets or sets the minimum number of points that a dense region must contain to be considered a cluster
        /// during execution.
        /// </summary>
        /// <remarks>
        /// Values less than one cause <see cref="Execute"/> to fail when evaluating core distances because the
        /// algorithm expects a positive cluster size.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Potentially thrown by consumers prior to calling <see cref="Execute"/> to validate input; no internal validation is performed.</exception>
        public int MinClusterSize { get; set; } = 5;

        /// <summary>
        /// Gets the clusters identified by the last call to <see cref="Execute"/>, where each inner list contains
        /// the points that belong to the same cluster.
        /// </summary>
        /// <remarks>
        /// The property is populated after <see cref="Execute"/> completes. It is <c>null</c> until the first
        /// execution, or empty when no clusters meet the minimum size requirement.
        /// </remarks>
        public List<List<PointFWithContext<T>>> Clusters { get; private set; }

        /// <summary>
        /// Executes the clustering algorithm by computing core distances, constructing a mutual reachability graph,
        /// building a minimum spanning tree, and extracting dense components whose size exceeds the configured
        /// threshold.
        /// </summary>
        /// <remarks>
        /// The method updates <see cref="Clusters"/> with the resulting partition. Points that do not belong to a
        /// sufficiently dense region are excluded from the final clusters.
        /// </remarks>
        /// <exception cref="NullReferenceException">Thrown when <see cref="Points"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <see cref="MinClusterSize"/> is less than one, resulting in an invalid core distance index.
        /// </exception>
        public void Execute()
        {
            int n = Points.Count;
            if (n == 0)
            {
                Clusters = new List<List<PointFWithContext<T>>>();
                return;
            }

            // 1. Calcular Core Distances
            double[] coreDistances = new double[n];
            for (int i = 0; i < n; i++)
            {
                var dists = new List<double>(n - 1);
                for (int j = 0; j < n; j++)
                {
                    if (i == j) continue;
                    dists.Add(Points[i].Point.EuclideanDistance(Points[j].Point));
                }
                dists.Sort();
                int index = MinClusterSize - 1;
                if (index >= dists.Count)
                    index = dists.Count - 1;
                coreDistances[i] = dists[index];
            }

            // 2. Calcular matriz de distancias mutuas de alcance
            double[,] mutualReachability = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = i; j < n; j++)
                {
                    if (i == j)
                        mutualReachability[i, j] = 0;
                    else
                    {
                        double dist = Points[i].Point.EuclideanDistance(Points[j].Point);
                        double reachDist = System.Math.Max(coreDistances[i], System.Math.Max(coreDistances[j], dist));
                        mutualReachability[i, j] = reachDist;
                        mutualReachability[j, i] = reachDist;
                    }
                }

            // 3. Construir MST usando Prim
            List<Edge> mst = BuildMST(mutualReachability);

            // 4. Construir jerarquía (simplificada, agrupando por corte de peso)
            Clusters = ExtractClustersFromMST(mst, n);
        }

        private List<Edge> BuildMST(double[,] distances)
        {
            int n = distances.GetLength(0);
            var mst = new List<Edge>();
            bool[] inMST = new bool[n];
            double[] key = new double[n];
            int[] parent = new int[n];

            for (int i = 0; i < n; i++)
            {
                key[i] = double.MaxValue;
                parent[i] = -1;
            }

            key[0] = 0;

            for (int count = 0; count < n - 1; count++)
            {
                double min = double.MaxValue;
                int u = -1;

                for (int v = 0; v < n; v++)
                {
                    if (!inMST[v] && key[v] < min)
                    {
                        min = key[v];
                        u = v;
                    }
                }

                inMST[u] = true;

                for (int v = 0; v < n; v++)
                {
                    if (!inMST[v] && distances[u, v] < key[v])
                    {
                        key[v] = distances[u, v];
                        parent[v] = u;
                    }
                }
            }

            for (int i = 1; i < n; i++)
            {
                mst.Add(new Edge(parent[i], i, distances[i, parent[i]]));
            }

            return mst;
        }

        private List<List<PointFWithContext<T>>> ExtractClustersFromMST(List<Edge> mst, int n)
        {
            // Para simplificar, cortamos las aristas más largas para formar clusters
            // Cortamos aristas cuyo peso es mayor que un umbral definido (por ejemplo, el percentil 75)
            var weights = mst.Select(e => e.Weight).OrderBy(w => w).ToList();
            double threshold = weights[(int)(weights.Count * 0.75)];

            // Construimos grafos conectados sin aristas > threshold
            var graph = new DisjointSet(n);
            foreach (var edge in mst)
            {
                if (edge.Weight <= threshold)
                    graph.Union(edge.Start, edge.End);
            }

            var clustersDict = new Dictionary<int, List<PointFWithContext<T>>>();
            for (int i = 0; i < n; i++)
            {
                int root = graph.Find(i);
                if (!clustersDict.ContainsKey(root))
                    clustersDict[root] = new List<PointFWithContext<T>>();
                clustersDict[root].Add(Points[i]);
            }

            // Filtrar clusters pequeños
            var clusters = clustersDict.Values
                .Where(c => c.Count >= MinClusterSize)
                .ToList();

            return clusters;
        }

        private class Edge
        {
            public int Start;
            public int End;
            public double Weight;

            public Edge(int s, int e, double w)
            {
                Start = s;
                End = e;
                Weight = w;
            }
        }

        private class DisjointSet
        {
            private int[] parent;
            private int[] rank;

            public DisjointSet(int size)
            {
                parent = new int[size];
                rank = new int[size];
                for (int i = 0; i < size; i++) parent[i] = i;
            }

            public int Find(int x)
            {
                if (parent[x] != x)
                    parent[x] = Find(parent[x]);
                return parent[x];
            }

            public void Union(int x, int y)
            {
                int rx = Find(x);
                int ry = Find(y);
                if (rx == ry) return;
                if (rank[rx] < rank[ry]) parent[rx] = ry;
                else if (rank[ry] < rank[rx]) parent[ry] = rx;
                else
                {
                    parent[ry] = rx;
                    rank[rx]++;
                }
            }
        }
    }
}

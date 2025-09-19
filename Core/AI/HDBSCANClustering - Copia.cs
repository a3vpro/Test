using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VisionNet.Core.Dawing;

namespace VisionNet.Core.AI
{
    /// <summary>
    /// Provides a simplified implementation of the HDBSCAN clustering algorithm for two-dimensional points
    /// associated with contextual metadata. The clustering process groups points into stable clusters based on
    /// mutual reachability distances and a configurable minimum cluster size.
    /// </summary>
    /// <typeparam name="T">The type of contextual information that accompanies each clustered point.</typeparam>
    public class HDBSCANClustering<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HDBSCANClustering{T}"/> class with default configuration
        /// values. The <see cref="Points"/> collection is initially empty and <see cref="MinClusterSize"/> is set to five.
        /// </summary>
        public HDBSCANClustering()
        {
        }

        /// <summary>
        /// Gets or sets the collection of points to be clustered. Each entry must contain a valid point and associated
        /// context, and the collection must not be <see langword="null"/> when <see cref="Execute"/> is invoked.
        /// </summary>
        /// <value>A mutable list of points that will be grouped into clusters.</value>
        public List<PointFWithContext<T>> Points { get; set; } = new List<PointFWithContext<T>>();

        /// <summary>
        /// Gets or sets the minimum number of points required to form a cluster. The value must be greater than zero
        /// and should not exceed the number of available points to avoid degenerate results.
        /// </summary>
        /// <value>The cluster size threshold used when condensing the hierarchy.</value>
        public int MinClusterSize { get; set; } = 5;

        /// <summary>
        /// Gets the collection of clusters detected during the most recent execution. The property remains
        /// <see langword="null"/> until <see cref="Execute"/> completes successfully.
        /// </summary>
        /// <value>A list containing the stable clusters extracted from the condensed hierarchy, or <see langword="null"/> if clustering has not been executed.</value>
        public List<List<PointFWithContext<T>>> Clusters { get; private set; }

        /// <summary>
        /// Performs the HDBSCAN clustering workflow on the configured <see cref="Points"/>, producing clusters that
        /// satisfy the <see cref="MinClusterSize"/> constraint. The method builds mutual reachability distances,
        /// constructs a minimum spanning tree, derives the cluster hierarchy, and extracts stable clusters.
        /// </summary>
        /// <exception cref="NullReferenceException">Thrown when <see cref="Points"/> is <see langword="null"/>, as the algorithm requires access to the point collection.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when <see cref="Points"/> is empty or when <see cref="MinClusterSize"/> is configured to a value less than one, producing invalid index lookups during tree construction or core distance evaluation.</exception>
        public void Execute()
        {
            int n = Points.Count;
            var mutualReachability = new double[n, n];

            // Calcular la distancia mutua de alcance
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                {
                    double dist = Distance(Points[i].Point, Points[j].Point);
                    double reachDist = System.Math.Max(dist, System.Math.Max(CoreDistance(i), CoreDistance(j)));
                    mutualReachability[i, j] = reachDist;
                    mutualReachability[j, i] = reachDist;
                }

            // Construir el árbol de expansión mínima (MST)
            var mst = MinimumSpanningTree(mutualReachability);

            // Construir la jerarquía de clusters
            var clusterHierarchy = BuildClusterHierarchy(mst);

            // Condensar la jerarquía
            var condensedHierarchy = CondenseHierarchy(clusterHierarchy);

            // Extraer clusters estables
            Clusters = ExtractStableClusters(condensedHierarchy);
        }

        private double CoreDistance(int pointIdx)
        {
            var distances = Points.Select(p => Distance(Points[pointIdx].Point, p.Point)).OrderBy(d => d).ToArray();
            return distances[System.Math.Min(MinClusterSize - 1, distances.Length - 1)];
        }

        private double Distance(PointF a, PointF b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        private List<Edge> MinimumSpanningTree(double[,] distances)
        {
            int n = distances.GetLength(0);
            var mst = new List<Edge>();
            var visited = new bool[n];
            var minEdge = new double[n];
            var parent = new int[n];
            for (int i = 0; i < minEdge.Length; i++)
                minEdge[i] = double.MaxValue;
            minEdge[0] = 0;
            parent[0] = -1;

            for (int count = 0; count < n - 1; count++)
            {
                double min = double.MaxValue;
                int u = -1;
                for (int v = 0; v < n; v++)
                {
                    if (!visited[v] && minEdge[v] < min)
                    {
                        min = minEdge[v];
                        u = v;
                    }
                }

                visited[u] = true;

                for (int v = 0; v < n; v++)
                {
                    if (!visited[v] && distances[u, v] < minEdge[v])
                    {
                        parent[v] = u;
                        minEdge[v] = distances[u, v];
                    }
                }
            }

            for (int i = 1; i < n; i++)
                mst.Add(new Edge(parent[i], i, distances[i, parent[i]]));

            return mst;
        }

        private List<ClusterNode> BuildClusterHierarchy(List<Edge> mst)
        {
            var nodes = mst.Select(e => new ClusterNode { PointIndex = e.Start, Parent = null, Children = new List<ClusterNode>() }).ToList();
            foreach (var edge in mst)
            {
                var node1 = nodes[edge.Start];
                var node2 = nodes[edge.End];
                node1.Children.Add(node2);
                node2.Parent = node1;
            }
            return nodes;
        }

        private List<ClusterNode> CondenseHierarchy(List<ClusterNode> hierarchy)
        {
            return hierarchy.Where(n => n.Children.Count >= MinClusterSize).ToList();
        }

        private List<List<PointFWithContext<T>>> ExtractStableClusters(List<ClusterNode> condensedHierarchy)
        {
            var clusters = new List<List<PointFWithContext<T>>>();
            foreach (var node in condensedHierarchy)
            {
                var cluster = new List<PointFWithContext<T>>();
                TraverseCluster(node, cluster);
                clusters.Add(cluster);
            }
            return clusters;
        }

        private void TraverseCluster(ClusterNode node, List<PointFWithContext<T>> cluster)
        {
            cluster.Add(Points[node.PointIndex]);
            foreach (var child in node.Children)
                TraverseCluster(child, cluster);
        }

        private class Edge
        {
            public int Start { get; }
            public int End { get; }
            public double Weight { get; }

            public Edge(int start, int end, double weight)
            {
                Start = start;
                End = end;
                Weight = weight;
            }
        }

        private class ClusterNode
        {
            public int PointIndex { get; set; }
            public ClusterNode Parent { get; set; }
            public List<ClusterNode> Children { get; set; }
        }
    }
}

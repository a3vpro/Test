using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using VisionNet.Core.Dawing;

namespace VisionNet.Core.AI
{
    /// <summary>
    /// Implements K-Means clustering using ML.NET over 2D points with context.
    /// </summary>
    /// <typeparam name="T">Type of additional context stored with each point.</typeparam>
    public class KMeansClustering<T>
    {
        public List<PointFWithContext<T>> Points { get; set; } = new List<PointFWithContext<T>>();
        public int ClusterCount { get; set; } = 3;
        public int MaxIterations { get; set; } = 100;
        public List<List<PointFWithContext<T>>> Clusters { get; private set; } = new List<List<PointFWithContext<T>>>();

        public void AddPoint(PointFWithContext<T> point) => Points.Add(point);
        public void Clear()
        {
            Points.Clear();
            Clusters.Clear();
        }

        /// <summary>
        /// Executes K-Means clustering via ML.NET.
        /// </summary>
        public void Execute()
        {
            if (ClusterCount < 1)
                throw new InvalidOperationException("ClusterCount must be at least 1.");
            if (Points.Count < ClusterCount)
                throw new InvalidOperationException("Not enough points to form the requested number of clusters.");

            var mlContext = new MLContext();
            // Prepare data
            var data = mlContext.Data.LoadFromEnumerable(
                Points.Select(p => new DataPoint { X = p.Point.X, Y = p.Point.Y })
            );
            // Pipeline: assemble features and train
            var pipeline = mlContext.Transforms.Concatenate("Features", nameof(DataPoint.X), nameof(DataPoint.Y))
                .Append(mlContext.Clustering.Trainers.KMeans(new KMeansTrainer.Options
                {
                    FeatureColumnName = "Features",
                    NumberOfClusters = ClusterCount,
                    MaximumNumberOfIterations = MaxIterations
                }));

            // Train model
            var model = pipeline.Fit(data);
            // Predict cluster assignments
            var predictions = model.Transform(data);
            var preds = mlContext.Data.CreateEnumerable<Prediction>(predictions, reuseRowObject: false).ToArray();

            // Build clusters
            Clusters = Enumerable.Range(0, ClusterCount)
                .Select(_ => new List<PointFWithContext<T>>())
                .ToList();
            for (int i = 0; i < Points.Count; i++)
            {
                // PredictedLabel is 1-based
                int clusterId = (int)preds[i].PredictedLabel - 1;
                Clusters[clusterId].Add(Points[i]);
            }
        }

        /// <summary>
        /// Executes minibatch K-Means by training on random subsets via ML.NET.
        /// </summary>
        public void ExecuteMiniBatch(int batchSize = 5000)
        {
            if (ClusterCount < 1)
                throw new InvalidOperationException("ClusterCount must be at least 1.");
            if (Points.Count < ClusterCount)
                throw new InvalidOperationException("Not enough points to form the requested number of clusters.");

            var mlContext = new MLContext();
            // Initialize accumulator for centroids
            PointF[] centroids = null;

            var rnd = new Random();
            for (int iter = 0; iter < MaxIterations; iter++)
            {
                // Sample a minibatch
                var batch = Points.OrderBy(_ => rnd.Next()).Take(batchSize).ToList();
                var batchData = mlContext.Data.LoadFromEnumerable(
                    batch.Select(p => new DataPoint { X = p.Point.X, Y = p.Point.Y })
                );
                // Train on batch
                var pipe = mlContext.Transforms.Concatenate("Features", nameof(DataPoint.X), nameof(DataPoint.Y))
                    .Append(mlContext.Clustering.Trainers.KMeans(new KMeansTrainer.Options
                    {
                        FeatureColumnName = "Features",
                        NumberOfClusters = ClusterCount,
                        MaximumNumberOfIterations = 1
                    }));
                var model = pipe.Fit(batchData);
                var modelParams = model.LastTransformer.Model;
                // Extract centroids
                VBuffer<float>[] centroidBuffers = null;
                modelParams.GetClusterCentroids(ref centroidBuffers, out int k);
                var batchCentroids = centroidBuffers
                    .Take(k)
                    .Select(vb => vb.DenseValues().ToArray())
                    .Select(arr => new PointF(arr[0], arr[1]))
                    .ToArray();

                // Accumulate centroids
                if (centroids == null)
                {
                    centroids = batchCentroids;
                }
                else
                {
                    for (int c = 0; c < ClusterCount; c++)
                    {
                        // running average
                        centroids[c] = new PointF(
                            (centroids[c].X * iter + batchCentroids[c].X) / (iter + 1),
                            (centroids[c].Y * iter + batchCentroids[c].Y) / (iter + 1)
                        );
                    }
                }
            }

            // Assign points to final centroids
            Clusters = Enumerable.Range(0, ClusterCount)
                .Select(_ => new List<PointFWithContext<T>>())
                .ToList();
            for (int i = 0; i < Points.Count; i++)
            {
                var pt = Points[i].Point;
                int best = 0;
                float minDist = DistanceSquared(pt, centroids[0]);
                for (int c = 1; c < ClusterCount; c++)
                {
                    float d = DistanceSquared(pt, centroids[c]);
                    if (d < minDist)
                    {
                        minDist = d; best = c;
                    }
                }
                Clusters[best].Add(Points[i]);
            }
        }

        private static float DistanceSquared(PointF a, PointF b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        // ML.NET data structures
        private class DataPoint
        {
            [LoadColumn(0)] public float X { get; set; }
            [LoadColumn(1)] public float Y { get; set; }
        }
        private class Prediction
        {
            [ColumnName("PredictedLabel")] public uint PredictedLabel { get; set; }
        }
    }
}

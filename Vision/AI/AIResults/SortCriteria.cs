using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using VisionNet.Drawing;

namespace VisionNet.Vision.AI
{
    public struct SortCriteria
    {
        public bool Enabled { get; set; }
        /// <summary>
        /// Use nominal of X and Y to measure the euclidean distance to the point
        /// </summary>
        public SingleSortCriteria EuclideanDistance { get; set; }
        public SingleSortCriteria X { get; set; }
        public SingleSortCriteria Y { get; set; }
        public SingleSortCriteria Size { get; set; }
        public SingleSortCriteria Score { get; set; }
        public SingleSortCriteria Area { get; set; }

        public double CalcWeight(double x, double y, double size, double score, double area)
        {
            double result = 0;

            if (Enabled)
            {
                double totalFactor = 0;

                if (EuclideanDistance.Enabled)
                {
                    PointF position = new PointF((float)x, (float)y);
                    PointF center = new PointF((float)X.Nominal, (float)Y.Nominal); // Take center from X and Y nominals
                    double dist = center.EuclideanDistance(position);                    
                    result += EuclideanDistance.CalcWeight(dist);
                    totalFactor += EuclideanDistance.Weight;
                }
                else if (X.Enabled)
                {
                    result += X.CalcWeight(x);
                    totalFactor += X.Weight;
                }
                else if (Y.Enabled)
                {
                    result += Y.CalcWeight(y);
                    totalFactor += Y.Weight;
                }

                if (Size.Enabled)
                {
                    result += Size.CalcWeight(size);
                    totalFactor += Size.Weight;
                }

                if (Score.Enabled)
                {
                    result += Score.CalcWeight(score);
                    totalFactor += Score.Weight;
                }

                if (Area.Enabled)
                {
                    result += Area.CalcWeight(area);
                    totalFactor += Area.Weight;
                }

                if (totalFactor == 0)
                    throw new InvalidOperationException("Sum of sorting weights should be more than zero.");

                result *= 1 / totalFactor;
            }

            return result;
        }

        public static SortCriteria Default
        {
            get
            {
                var result = new SortCriteria();
                result.Enabled = false;
                result.EuclideanDistance = SingleSortCriteria.Default;
                result.X = SingleSortCriteria.Default;
                result.Y = SingleSortCriteria.Default;
                result.Size = SingleSortCriteria.Default;
                result.Score = SingleSortCriteria.Default;
                result.Area = SingleSortCriteria.Default;
                return result;
            }
        }
    }
}

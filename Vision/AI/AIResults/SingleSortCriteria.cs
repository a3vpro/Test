using System;
using System.Collections.Generic;
using System.Text;

namespace VisionNet.Vision.AI
{
    public struct SingleSortCriteria
    {
        public bool Enabled { get; set; }
        public double Nominal { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public SortMode SortMode { get; set; }
        public bool Inverse { get; set; }
        public double Weight { get; set; }

        public double CalcWeight(double value)
        {
            double result = 0;

            if (Min >= Max)
            {
                throw new InvalidOperationException("Min must be less than Max.");
            }

            switch (SortMode)
            {
                case SortMode.Linear_MinMax:
                    if (value <= Min)
                        result = 0;
                    else if (value >= Max)
                        result = 1;
                    else
                        result = (value - Min) / (Max - Min);
                    break;

                case SortMode.Triangle_MinNominalMax:
                    if (SortMode == SortMode.Triangle_MinNominalMax && (Nominal <= Min || Nominal >= Max))
                    {
                        throw new InvalidOperationException("Nominal must be between Min and Max.");
                    }

                    if (value <= Min || value >= Max)
                        result = 0;
                    else if (value == Nominal)
                        result = 1;
                    else if (value < Nominal)
                        result = (value - Min) / (Nominal - Min);
                    else
                        result = (Max - value) / (Max - Nominal);
                    break;
                default:
                    result = 0;
                    break;
            }

            if (Inverse)
            {
                result = 1 / result;
            }

            return result * Weight;
        }

        public static SingleSortCriteria Default
        {
            get
            {
                var result = new SingleSortCriteria();
                result.Enabled = false;
                result.Nominal = 0;
                result.Weight = 0;
                result.SortMode = SortMode.Linear_MinMax;
                result.Inverse = false;
                return result;
            }
        }
    }
}

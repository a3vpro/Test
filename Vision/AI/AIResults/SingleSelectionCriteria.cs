using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisionNet.Vision.AI
{
    public struct SingleSelectionCriteria
    {
        public bool Enabled { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double WarningMin { get; set; }
        public double WarningMax { get; set; }
        public SelectMode SelectMode { get; set; }

        public SelectionStatus FitCriteria(double value)
        {
            SelectionStatus result = SelectionStatus.Unselected;

            switch (SelectMode)
            {
                case SelectMode.LowerThanMax:
                    if (value <= Max)
                    {
                        result = SelectionStatus.Selected;
                    }
                    else
                    {
                        if (value <= WarningMax)
                        {
                            result = SelectionStatus.Warning;
                        }
                        else
                        {
                            result = SelectionStatus.Unselected;
                        }
                    }

                    break;
                case SelectMode.UpperThanMin:
                    if (value >= Min)
                    {
                        result = SelectionStatus.Selected;
                    }
                    else
                    {
                        if (value >= WarningMin)
                        {
                            result = SelectionStatus.Warning;
                        }
                        else
                        {
                            result = SelectionStatus.Unselected;
                        }
                    }

                    break;
                case SelectMode.BetweenMinMax:
                    if (value >= Min && value <= Max)
                    {
                        result = SelectionStatus.Selected;
                    }
                    else
                    {
                        if (value >= WarningMin && value <= WarningMax)
                        {
                            result = SelectionStatus.Warning;
                        }
                        else
                        {
                            result = SelectionStatus.Unselected;
                        }
                    }

                    break;
                case SelectMode.OutsideMinMax:
                    if (value >= Max || value <= Min)
                    {
                        result = SelectionStatus.Selected;
                    }
                    else
                    {
                        if (value >= WarningMax || value <= WarningMin)
                        {
                            result = SelectionStatus.Warning;
                        }
                        else
                        {
                            result = SelectionStatus.Unselected;
                        }
                    }

                    break;
                default:
                    break;
            }

            return result;
        }

        public static SingleSelectionCriteria Default
        {
            get
            {
                var result = new SingleSelectionCriteria();
                result.Enabled = false;
                result.Min = 0;
                result.Max = 0;
                result.WarningMin = 0;
                result.WarningMax = 0;
                result.SelectMode = SelectMode.BetweenMinMax;
                return result;
            }
        }
    }
}

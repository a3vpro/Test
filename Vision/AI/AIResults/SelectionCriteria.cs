using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using VisionNet.Drawing;

namespace VisionNet.Vision.AI
{
    public struct SelectionCriteria
    {
        public bool Enabled { get; set; }
        public SingleSelectionCriteria Size { get; set; }
        public SingleSelectionCriteria Score { get; set; }
        public SingleSelectionCriteria Area { get; set; }
        public SingleSelectionCriteria Weight { get; set; }


        public SelectionStatus FitCriteria(double size, double score, double area, double weight)
        {
            SelectionStatus result = SelectionStatus.Selected;

            if (Enabled)
            {
                if (Size.Enabled)
                {
                    SelectionStatus sizeResult = Size.FitCriteria(size);
                    result = CombineStatuses(result, sizeResult);
                }

                if (Score.Enabled)
                {
                    SelectionStatus scoreResult = Score.FitCriteria(score);
                    result = CombineStatuses(result, scoreResult);
                }

                if (Area.Enabled)
                {
                    SelectionStatus areaResult = Area.FitCriteria(area);
                    result = CombineStatuses(result, areaResult);
                }

                if (Weight.Enabled)
                {
                    SelectionStatus weightResult = Weight.FitCriteria(weight);
                    result = CombineStatuses(result, weightResult);
                }
            }

            return result;
        }

        private SelectionStatus CombineStatuses(SelectionStatus current, SelectionStatus next)
        {
            SelectionStatus combined = SelectionStatus.Selected;

            if (current == SelectionStatus.Unselected || next == SelectionStatus.Unselected)
            {
                combined = SelectionStatus.Unselected;
            }
            else if (current == SelectionStatus.Warning || next == SelectionStatus.Warning)
            {
                combined = SelectionStatus.Warning;
            }

            return combined;
        }

        public static SelectionCriteria Default
        {
            get
            {
                var result = new SelectionCriteria();
                result.Enabled = false;
                result.Size = SingleSelectionCriteria.Default;
                result.Score = SingleSelectionCriteria.Default;
                result.Area = SingleSelectionCriteria.Default;
                result.Weight = SingleSelectionCriteria.Default;
                return result;
            }
        }
    }
}

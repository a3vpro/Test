//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 23-06-2023
// Description      : v1.7.0
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.            
//-----------------------------------------------
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VisionNet.Core.Types;
using VisionNet.Vision.AI;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Represents the result of an inspection process.
    /// </summary>
    [Serializable]
    public class InspectionResult : ICloneable
    {
        /// <summary>
        /// Order of the inspection. Internal use only.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public int Order { get; set; } = 0;

        /// <summary>
                                                            /// Gets or sets the name of the inspection.
                                                            /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Name of the inspection.")]
        [DisplayName(nameof(Name))]
        public string Name { get; set; } = "NoName";

        /// <summary>
        /// Gets or sets a value indicating whether the inspection is enabled.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("The inspection is enabled / disabled.")]
        [DisplayName(nameof(Enabled))]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the result of the inspection.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Result of the inspection.")]
        [DisplayName(nameof(Result))]
        public bool Result { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the inspection executed successfully.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Inspection executed successfully.")]
        [DisplayName(nameof(Success))]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the previous result of the inspection.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Previous result of the inspection.")]
        [DisplayName(nameof(PrevResult))]
        public bool PrevResult { get; set; }

        /// <summary>
        /// Gets or sets the previous result of the inspection.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Warning status of the inspection.")]
        [DisplayName(nameof(Warning))]
        public bool Warning { get; set; }

        /// <summary>
        /// Gets the status of the inspection result.
        /// </summary>
        [Browsable(false)]
        public ResultStatus Status => ResultStatusHelper.Create(Result, Enabled, Success, PrevResult, Warning);

        /// <summary>
        /// Gets or sets the duration of the execution in milliseconds.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Duration of the execution in milliseconds.")]
        [DisplayName(nameof(ProcessTime))]
        public double ProcessTime { get; set; }

        /// <summary>
        /// Gets or sets the error description of the inspection.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Error description.")]
        [DisplayName(nameof(Error))]
        public string Error { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether the inspection is included in calculating the result.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("The inspection is included in calculating the result.")]
        [DisplayName(nameof(IncludeInResult))]
        public bool IncludeInResult { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the inspection is included in statistics.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("The inspection is included in statistics.")]
        [DisplayName(nameof(IncludeInStats))]
        public bool IncludeInStats { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the inspection is separated by class type result,
        /// commonly used in deep learning segmentation results.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("The inspection is separated by the class type result. Usually used in DL segmentation results.")]
        [DisplayName(nameof(IsSegmentationInspection))]
        public bool IsSegmentationInspection { get; set; } = false;

        /// <summary>
        /// Gets or sets the list of all measurable values in the inspection.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("List of all measurables.")]
        [DisplayName(nameof(Measurables))]
        public List<NamedValue> Measurables { get; set; } = new List<NamedValue>();

        /// <summary>
        /// Gets or sets the information about the inspection step.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("InspectionResult")]
        [Description("Information about the inspection step.")]
        [DisplayName(nameof(Step))]
        public List<NamedValue> Step { get; set; } = new List<NamedValue>();

        /// <summary>
        /// Populates a list of inspection results, separating segmented inspections when applicable.
        /// </summary>
        /// <returns>A list of <see cref="InspectionResult"/> objects.</returns>
        public List<InspectionResult> Populate()
        {
            var result = new List<InspectionResult> {};

            //result.Add(this); Comentado por no necesitarse. Se excluye el resultado actual

            if (IsSegmentationInspection)
            {
                var instSegmentationInferenceResult = this.Measurables
                    .Select(m => m.Value)
                    .OfType<List<SingleInstSegmentationInferenceResult>>()
                    .SelectMany(list => list);

                int count = 0;
                var segmentedResults = instSegmentationInferenceResult.Select(i =>
                {
                    var inspectionResult = new InspectionResult
                    {
                        Order = count,
                        Name = $"{this.Name}.{i.ExtraInfo.Alias}",
                        Enabled = this.Enabled,
                        Result = i.Result,
                        Success = this.Success,
                        Warning = i.Selected == SelectionStatus.Warning,
                        PrevResult = this.PrevResult,
                        ProcessTime = this.ProcessTime,
                        IncludeInResult = this.IncludeInResult,
                        IncludeInStats = this.IncludeInStats,
                        IsSegmentationInspection = this.IsSegmentationInspection,
                        Measurables = new List<NamedValue>
                        {
                            new NamedValue { Name = "Instance", Type = BasicTypeCode.Object, Value = i },
                            new NamedValue { Name = "ClassIndex", Type = BasicTypeCode.IntegerNumber, Value = i.ClassIndex },
                            new NamedValue { Name = "ClassName", Type = BasicTypeCode.String, Value = i.ClassName },
                            //new NamedValue { Name = "Description", Type = BasicTypeCode.String, Value = i.ExtraInfo.Description },
                            //new NamedValue { Name = "Alias", Type = BasicTypeCode.String, Value = i.ExtraInfo.Alias },
                            //new NamedValue { Name = "Group", Type = BasicTypeCode.String, Value = i.ExtraInfo.Group },
                            //new NamedValue { Name = "Icon", Type = BasicTypeCode.String, Value = i.ExtraInfo.Icon },
                            //new NamedValue { Name = "Color", Type = BasicTypeCode.Object, Value = i.ExtraInfo.Color },
                            new NamedValue { Name = "Score", Type = BasicTypeCode.FloatingPointNumber, Value = i.Score },
                            new NamedValue { Name = "Size", Type = BasicTypeCode.FloatingPointNumber, Value = i.BoundingBox.Diagonal },
                            //new NamedValue { Name = "Center", Type = BasicTypeCode.Object, Value = i.Center },
                            //new NamedValue { Name = "CenterX", Type = BasicTypeCode.FloatingPointNumber, Value = i.Center.X },
                            //new NamedValue { Name = "CenterY", Type = BasicTypeCode.FloatingPointNumber, Value = i.Center.Y },
                            //new NamedValue { Name = "BoundingBox", Type = BasicTypeCode.Object, Value = i.BoundingBox },
                            //new NamedValue { Name = "BoundingBoxX", Type = BasicTypeCode.FloatingPointNumber, Value = i.BoundingBox?.X },
                            //new NamedValue { Name = "BoundingBoxY", Type = BasicTypeCode.FloatingPointNumber, Value = i.BoundingBox?.Y },
                            //new NamedValue { Name = "BoundingBoxWidth", Type = BasicTypeCode.FloatingPointNumber, Value = i.BoundingBox?.Width },
                            //new NamedValue { Name = "BoundingBoxHeight", Type = BasicTypeCode.FloatingPointNumber, Value = i.BoundingBox?.Height },
                            //new NamedValue { Name = "BoundingBoxDiagonal", Type = BasicTypeCode.FloatingPointNumber, Value = i.BoundingBox?.Diagonal },
                            //new NamedValue { Name = "BoundingBoxAngle", Type = BasicTypeCode.FloatingPointNumber, Value = i.BoundingBox?.Angle },
                            //new NamedValue { Name = "Polygon", Type = BasicTypeCode.Object, Value = i.Polygon },
                            //new NamedValue { Name = "Area", Type = BasicTypeCode.FloatingPointNumber, Value = i.Polygon?.Area },
                            //new NamedValue { Name = "Position", Type = BasicTypeCode.Object, Value = i.Position },
                            new NamedValue { Name = "PositionX", Type = BasicTypeCode.FloatingPointNumber, Value = i.Position?.Origin.X },
                            new NamedValue { Name = "PositionY", Type = BasicTypeCode.FloatingPointNumber, Value = i.Position?.Origin.Y },
                            //new NamedValue { Name = "PositionAngle", Type = BasicTypeCode.FloatingPointNumber, Value = i.Position?.Angle },
                        },
                        Step = new List<NamedValue>(this.Step)
                    };

                    var nonSegmentationMeasurables = this.Measurables
                        .Where(m => !(m.Value is List<SingleInstSegmentationInferenceResult>));
                    inspectionResult.Measurables.AddRange(nonSegmentationMeasurables);

                    count++;

                    return inspectionResult;
                }).ToList();

                result.AddRange(segmentedResults);
            }
            else
            {
                result.Add(this);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the collection of output parameters.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public IOutputParametersCollection OutputParameters { get; set; } = new OutputParametersCollection();

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="InspectionResult"/> object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new InspectionResult
            {
                Name = Name,
                Enabled = Enabled,
                Result = Result,
                Success = Success,
                PrevResult = PrevResult,
                ProcessTime = ProcessTime,
                Warning = Warning,
                Error = Error,
                IncludeInResult = IncludeInResult,
                IncludeInStats = IncludeInStats,
                IsSegmentationInspection = IsSegmentationInspection,
                Measurables = Measurables?.Where(m => m != null).Select(m => m.Clone() as NamedValue).ToList(),
                Step = Step?.Where(s => s != null).Select(s => s.Clone() as NamedValue).ToList(),
                OutputParameters = OutputParameters
            };
        }
    }
}
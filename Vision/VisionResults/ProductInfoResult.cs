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
using System;
using System.ComponentModel;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    [Serializable]
    /// <summary>
    /// Represents the result of a product inspection process, including metadata and execution details.
    /// </summary>
    public class ProductInfoResult : ICloneable
    {
        /// <summary>
        /// Identification of the location of the system (conveyor, cell, factory...)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Identification of the location of the system (conveyor, cell, factory...).")]
        [DisplayName(nameof(Source))]
        public string Source { get; set; } = "NoLine";

        /// <summary>
        /// Indicates whether inspections are enabled or disabled.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("The inspections are enabled / disabled.")]
        [DisplayName(nameof(Enabled))]
        public bool Enabled { get; set; }

        /// <summary>
        /// Represents the result of the inspections.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Result of the inspections.")]
        [DisplayName(nameof(Result))]
        public bool Result { get; set; }

        /// <summary>
        /// Indicates whether inspections were successfully executed.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Inspections executed successfully.")]
        [DisplayName(nameof(Success))]
        public bool Success { get; set; }

        /// <summary>
        /// Indicates whether inspections have a warning inside.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Inspections executed with warning.")]
        [DisplayName(nameof(Warning))]
        public bool Warning { get; set; }

        /// <summary>
        /// Duration of the execution in milliseconds.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Duration of the execution in milliseconds.")]
        [DisplayName(nameof(ProcessTime))]
        public double ProcessTime { get; set; }

        /// <summary>
        /// Description of any error that occurred.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Error description.")]
        [DisplayName(nameof(Error))]
        public string Error { get; set; } = "";

        /// <summary>
        /// Time of the end of execution.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Time of the end of execution.")]
        [DisplayName(nameof(DateTime))]
        public DateTime DateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Indicates whether the system is in bypass mode.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Bypass mode.")]
        [DisplayName(nameof(ByPass))]
        public bool ByPass { get; set; }

        /// <summary>
        /// Indicates whether any inspection has a forced value.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Some inspection has a forced value.")]
        [DisplayName(nameof(Forced))]
        public bool Forced { get; set; }

        /// <summary>
        /// Unique of the piece or product.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Unique of the piece or product.")]
        [DisplayName(nameof(InternalIndex))]
        public long InternalIndex { get; set; }

        /// <summary>
        /// Unique of the piece or product.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("ProductInfoResult")]
        [Description("Unique Guid of the piece or product.")]
        [DisplayName(nameof(Id))]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the status of the inspection result.
        /// </summary>
        [Browsable(false)]
        public ResultStatus Status => ResultStatusHelper.Create(Result, Enabled, Success, true, Warning);

        /// <summary>
        /// Creates a deep copy of the current <see cref="ProductInfoResult"/> instance.
        /// </summary>
        /// <returns>A new instance of <see cref="ProductInfoResult"/> with the same data.</returns>
        public object Clone()
        {
            return new ProductInfoResult
            {
                Source = Source,
                Enabled = Enabled,
                Result = Result,
                Success = Success,
                ProcessTime = ProcessTime,
                Warning = Warning,
                Error = Error,
                DateTime = DateTime,
                ByPass = ByPass,
                Forced = Forced,
                InternalIndex = InternalIndex,
                Id = Id
            };
        }
    }
}

using System.ComponentModel;

namespace VisionNet.Core.Requisites
{
    /// <summary>
    /// Represents configuration options for an application requisite definition.
    /// </summary>
    public class ApplicationRequisitesOptions
    {
        /// <summary>
        /// Gets or sets the type of the requisite that determines how the requirement is evaluated.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Requisites")]
        [Description("Type of the requisite.")]
        [DisplayName(nameof(Type))]
        public RequisiteType Type { get; set; } = RequisiteType.Custom;

        /// <summary>
        /// Gets or sets the display name used to identify the requisite within the application configuration.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Requisites")]
        [Description("Name of the requisite.")]
        [DisplayName(nameof(Name))]
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the argument value supplied to the requisite handler, such as command-line parameters or identifiers.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Requisites")]
        [Description("Argument or parameter used by the requisite.")]
        [DisplayName(nameof(Argument))]
        public string Argument { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether the requisite should be evaluated when processing application requirements.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Requisites")]
        [Description("Enable or disable the requisite check.")]
        [DisplayName(nameof(Enabled))]
        public bool Enabled { get; set; }
    }
}

using System.ComponentModel;

namespace VisionNet.Core.Requisites
{
    public class ApplicationRequisitesOptions
    {
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Requisites")]
        [Description("Type of the requisite.")]
        [DisplayName(nameof(Type))]
        public RequisiteType Type { get; set; } = RequisiteType.Custom;

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Requisites")]
        [Description("Name of the requisite.")]
        [DisplayName(nameof(Name))]
        public string Name { get; set; } = "";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Requisites")]
        [Description("Argument or parameter used by the requisite.")]
        [DisplayName(nameof(Argument))]
        public string Argument { get; set; } = "";

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Requisites")]
        [Description("Enable or disable the requisite check.")]
        [DisplayName(nameof(Enabled))]
        public bool Enabled { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisionNet.DataBases
{
    [Table("Inspections")]
    public class StatisticsRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long ProductId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        public string InspectionName { get; set; }
        public bool MatchingCriteria { get; set; }

        //[Index(nameof(ProductId), nameof(DateTime))]
        //public static object CompositeIndex { get; } // Este atributo es para mostrar el propósito, ya que EF Core aplica índices en el modelo fluido.
    }
}

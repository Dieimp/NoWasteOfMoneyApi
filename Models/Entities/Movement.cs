using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace NoWasteOfMoney.Models.Entities
{
    public class Movement
    {

        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        [Required]
        public int MovementTypeId { get; set; }

        // Navigation Property - Fundamental para o mapeamento do EF Core
        public virtual MovementType MovementType { get; set; } = null!;
    }
}
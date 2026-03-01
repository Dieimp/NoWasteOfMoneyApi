using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Models.Entities
{
    public class MonthMovement
    {
        public Guid Id { get; set; }

        [Required]
        public Guid MovementId { get; set; }
        [Required]
        public Guid PersonId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }
        [Required]
        public decimal Value { get; set; }
        public virtual Movement Movement { get; set; } = null!;
        public virtual Person Person { get; set; } = null!;

    }
}
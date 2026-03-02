using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Models.Dtos
{
    public struct CreateMonthMovement
    {
        [Required(ErrorMessage = "O ID da movimentação é obrigatório.")]
        public Guid MovementId { get; init; }

        // [Required(ErrorMessage = "O ID da pessoa é obrigatório.")]
        // public int PersonId { get; init; }

        [Required(ErrorMessage = "O ID da pessoa é obrigatório.")]
        public DateOnly Date { get; init; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Value { get; init; }

    }
}
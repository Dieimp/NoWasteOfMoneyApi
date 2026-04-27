using System;
using System.ComponentModel.DataAnnotations;

namespace NoWasteOfMoney.Models.Dtos
{
    public struct UpdateMonthMovement
    {
        [Required(ErrorMessage = "O ID da movimentação é obrigatório.")]
        public Guid MovementId { get; init; }

        [Required(ErrorMessage = "A data é obrigatória.")]
        public DateOnly Date { get; init; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Value { get; init; }
    }
}

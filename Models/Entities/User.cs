using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NoWasteOfMoney.Models.Entities
{
    namespace NoWasteOfMoney.Domain.Entities
    {
        public class User
        {
            [Key]
            public Guid Id { get; set; } = Guid.NewGuid(); // ? Gerado no construtor

            [Required, ForeignKey("Person")]
            public Guid PersonId { get; set; }

            [Required, MaxLength(255)]
            public string PasswordHash { get; set; }

            [Required, MaxLength(50)]
            public string Role { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }

            [MaxLength(100)]
            public string? PasswordResetToken { get; set; }

            public DateTime? ResetTokenExpiresAt { get; set; }

            public virtual Person Person { get; set; } = null!;
        }
    }
}

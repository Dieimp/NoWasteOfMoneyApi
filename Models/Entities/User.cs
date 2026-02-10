using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NoWasteOfMoney.Models.Entities
{

    namespace NoWasteOfMoney.Domain.Entities
    {

        public class User
        {

            [Key]
            public int Id { get; set; }
            [Required]
            public int PersonId { get; set; }
            public string PasswordHash { get; set; }

            [Required]
            public string Role { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public virtual Person Person { get; set; } = null!;

        }
    }
}
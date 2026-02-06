using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NoWasteOfMoney.Models.Entities
{
    public class User
    {
        protected User() { }
        public User(string name, string email, string passwordHash, string role)
        {

            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            Active = true;
            CreatedAt = DateTime.UtcNow; // Sempre UTC
        }

        [Key]
        public int Id { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; private set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; private set; }

        [Required]
        public string PasswordHash { get; private set; }

        [Required]
        public string Role { get; private set; }

        public bool Active { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        [Required]
        public int PersonId { get; private set; }
        [ForeignKey("PersonId")]
        public virtual Person Person { get; private set; }


    }
}
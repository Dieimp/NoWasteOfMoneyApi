using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Models.Entities
{

    // Localizada em NoWasteOfMoney.Domain.Entities
    public class Person
    {
        // Construtor para o EF


        [Key]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(255)]
        public string LastName { get; set; }

        [Required, StringLength(255), EmailAddress]
        public string Email { get; set; }
    }

}

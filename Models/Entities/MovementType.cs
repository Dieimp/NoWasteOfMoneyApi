using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoWasteOfMoney.Models.Entities
{
    public class MovementType
    {
        public static readonly MovementType Debit = new MovementType(1, "Debit");
        public static readonly MovementType Credit = new MovementType(2, "Credit");

        public int Id { get; private set; }
        public string Name { get; private set; }

        private MovementType(int id, string name)
        {
            Id = id;
            Name = name;

        }
        // Propriedade de navegação inversa (Opcional, mas útil para consultas)
        public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();
        // Construtor privado para o EF Core
        private MovementType() { }

    }
}
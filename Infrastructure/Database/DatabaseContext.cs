using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NoWasteOfMoney.Models.Entities;

namespace NoWasteOfMoney.Infrastructure.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Person> Persons { get; set; }

        public DbSet<Movement> Movements { get; set; }
        public DbSet<MonthMovement> MonthMovements { get; set; }


        //Estudadr melhor
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasIndex(p => p.Email)
                .IsUnique();


            modelBuilder.Entity<MovementType>(entity =>
            {
                //Valores padroes da tabela auxiliar
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);

                entity.HasData(MovementType.Debit, MovementType.Credit);
            });

            // 2. Configuração da Entidade Movement (Sua solicitação corrigida)
            modelBuilder.Entity<Movement>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Configuração da FK para o Smart Enum
                entity.HasOne(d => d.MovementType)
                      .WithMany(p => p.Movements) // Se você não tiver a coleção no MovementType, use .WithMany() vazio
                      .HasForeignKey(d => d.MovementTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Índices adicionais para performance
                entity.HasIndex(e => e.MovementTypeId);
            });

            modelBuilder.Entity<MonthMovement>(entity =>
            {
                entity.HasKey(e => e.Id);


                entity.HasOne(d => d.Movement)
                      .WithMany()
                      .HasForeignKey(d => d.MovementId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Person)
                      .WithMany()
                      .HasForeignKey(d => d.PersonId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.Month, e.Year });
            });

        }

    }
}
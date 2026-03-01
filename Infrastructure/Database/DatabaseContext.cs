using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NoWasteOfMoney.Models.Entities;
using NoWasteOfMoney.Models.Entities.NoWasteOfMoney.Domain.Entities;

namespace NoWasteOfMoney.Infrastructure.Database
{
    public class DatabaseContext : DbContext
    {
        // Fixed seed GUIDs — stable across all migrations
        public static readonly Guid SeedPersonId    = new Guid("11111111-0000-0000-0000-000000000001");
        public static readonly Guid SeedMovement1Id = new Guid("22222222-0000-0000-0000-000000000001");
        public static readonly Guid SeedMovement2Id = new Guid("22222222-0000-0000-0000-000000000002");
        public static readonly Guid SeedMonthMovId  = new Guid("33333333-0000-0000-0000-000000000001");

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Person> Persons { get; set; }

        public DbSet<Movement> Movements { get; set; }
        public DbSet<MonthMovement> MonthMovements { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Person ──────────────────────────────────────────────────────────
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasIndex(p => p.Email).IsUnique();

                entity.HasData(new Person
                {
                    Id        = SeedPersonId,
                    FirstName = "Pessoa",
                    LastName  = "Top",
                    Email     = "adimin@semPerdaDeDinheiro.com"
                });
            });

            // ── MovementType (smart enum — int PKs stay as-is) ──────────────────
            modelBuilder.Entity<MovementType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasData(MovementType.Debit, MovementType.Credit);
            });

            // ── Movement ────────────────────────────────────────────────────────
            modelBuilder.Entity<Movement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.MovementType)
                      .WithMany(p => p.Movements)
                      .HasForeignKey(d => d.MovementTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.MovementTypeId);

                entity.HasData(
                    new Movement
                    {
                        Id             = SeedMovement1Id,
                        Name           = "Academia",
                        Description    = "Ficar grande",
                        MovementTypeId = 1
                    },
                    new Movement
                    {
                        Id             = SeedMovement2Id,
                        Name           = "Pos graduacao",
                        Description    = "Receba inteligencia",
                        MovementTypeId = 1
                    }
                );
            });

            // ── MonthMovement ───────────────────────────────────────────────────
            modelBuilder.Entity<MonthMovement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Movement)
                      .WithMany()
                      .HasForeignKey(d => d.MovementId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Person)
                      .WithMany()
                      .HasForeignKey(d => d.PersonId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.Month, e.Year });

                entity.HasData(new MonthMovement
                {
                    Id         = SeedMonthMovId,
                    MovementId = SeedMovement1Id,
                    PersonId   = SeedPersonId,
                    Year       = 2026,
                    Month      = 2,
                    Value      = 0
                });
            });
        }
    }
}
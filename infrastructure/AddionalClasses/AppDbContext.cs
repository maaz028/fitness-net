using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using core.Models;
using core.Models.TrainerModels;
using core.Models.EquipmentModels;

namespace infrastructure.AddionalClasses
{
    public class AppDbContext : IdentityDbContext<ApplicationUserModel>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
         

        }

        public DbSet<TrainerModel>? Trainer { get; set; }   
        public DbSet<EquipmentModel>? Equipment { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TrainerModel>()
               .HasIndex(p => new { p.Email })
               .IsUnique(true);


            foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
            }
        }

    }
}

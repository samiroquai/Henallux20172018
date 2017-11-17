using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectUsingDbFirst
{
    public partial class mdpschsa_labaccconcContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<ApplicationUser>
    {
        
        public mdpschsa_labaccconcContext(DbContextOptions<mdpschsa_labaccconcContext> options)
            :base(options)
        {
            
        }
        public virtual DbSet<Annonces> Annonces { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<GeoDemo> GeoDemo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                           }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Annonces>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Customers>(entity =>
            {
                entity.ToTable("Customers", "labo6");

                entity.Property(e => e.Email).HasColumnName("EMail");

                entity.Property(e => e.RowVersion).IsRowVersion();
            });

            modelBuilder.Entity<GeoDemo>(entity =>
            {
                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace model
{
    public partial class mdpschsa_labaccconcContext : DbContext
    {
        public virtual DbSet<GeoDemo> GeoDemo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                string connectionString=null;
                if(connectionString==null)
                    throw new Exception("Définissez la connection string :). Le nom de la DB est mdpschsa_labaccconc");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeoDemo>(entity =>
            {
                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}

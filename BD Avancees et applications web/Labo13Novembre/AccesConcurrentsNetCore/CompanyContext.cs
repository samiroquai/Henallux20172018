using Microsoft.EntityFrameworkCore;

namespace AccesConcurrentsNetCore
{
    public class CompanyContext: DbContext
    {
        public CompanyContext(DbContextOptions options)
        :base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("labo6");
    }

        public DbSet<Customer> Customers{get;set;}
    }
}
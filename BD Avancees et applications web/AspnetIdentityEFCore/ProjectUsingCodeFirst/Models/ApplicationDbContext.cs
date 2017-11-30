using System;
using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class ApplicationDbContext: Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options)
        :base(options)
        {
            this.Database.EnsureCreated();            
        }
    }
}
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DataDbContext : IdentityDbContext<User>
    {

        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) 
        {

        }

        public DbSet<Customer> Customers { get; set; }
    }
}

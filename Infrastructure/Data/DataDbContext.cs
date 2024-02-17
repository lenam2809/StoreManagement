using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DataDbContext : DbContext
    {

        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) 
        {

        }

        public DbSet<Customer> Customers { get; set; }
    }
}

using API_JWTToken_Products.Models;
using Microsoft.EntityFrameworkCore;

namespace API_JWTToken_Products.Context
{
    public class productContext : DbContext
    {
        public productContext(DbContextOptions<productContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

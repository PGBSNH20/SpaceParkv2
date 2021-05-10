using Microsoft.EntityFrameworkCore;
using SpacePark_API.Models;

namespace SpacePark_API.DataAccess
{
    public class StarwarsContext : DbContext
    {
        public StarwarsContext(DbContextOptions<StarwarsContext> options) : base(options) { }
        public DbSet<SpaceShip> SpaceShips { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Homeworld> Homeworlds { get; set; }
        public DbSet<SpacePort> SpacePorts { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
    }
}
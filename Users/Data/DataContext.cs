using Microsoft.EntityFrameworkCore;
using Users.Models;

/**
 * Data Context class
 * Here is configurade all the tables to be used with EntityFramework
 */
namespace Users.Data
{
    public class DataContext: DbContext
    {
        // Default constructor
        public DataContext() { }

        // Constructor to create an instance of DataContext class with specifc configuration
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // Set Table Users
        public DbSet<User> Users { get; set; }

        // Set Table Users
        public DbSet<LoginAttempts> LoginAttempts { get; set; }

        // Set Table Users
        public DbSet<OtpUser> OtpRequest { get; set; }
    }
}

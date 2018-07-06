using Microsoft.EntityFrameworkCore;

namespace ReviewBuilder.Models
{
    public class ApplicationContext : DbContext
    {
        private const string ConnectionString = "Data Source=data.db";

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { }
        public DbSet<User> UserModel { get; set; }
        //public DbSet<FileModel> Files { get; set; }
         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

    }
}
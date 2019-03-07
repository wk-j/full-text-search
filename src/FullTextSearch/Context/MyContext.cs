using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace FullTextSearch.Context {
    public class Student {
        [Key]
        public int Id { set; get; }
        public string Name { set; get; }
        public string Profile1 { set; get; }
        public string Profile2 { set; get; }
        public string Profile3 { set; get; }
        // public NpgsqlTsVector SearchVector { get; set; }
    }

    public class MyContext : DbContext {
        public MyContext(DbContextOptions options) : base(options) {

        }
        public DbSet<Student> Students { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // modelBuilder.Entity<Student>()
            //     .HasIndex(p => p.SearchVector)
            //     .ForNpgsqlHasMethod("GIN");
        }

    }
}
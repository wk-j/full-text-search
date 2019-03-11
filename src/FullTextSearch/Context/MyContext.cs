using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using System.Linq;

namespace FullTextSearch.Context {
    class Text {
        public static string[] A = new[] {
            "กตเวทิตา",
            "กตเวที",
            "กบฏ",
            "กรมขุน",
            "กรมคลัง",
            "กรมท่า",
            "กรมธรรม์",
            "กรมนา",
            "กรมพระ",
            "กรมวัง",
            "กรมเวียง",
            "กรมหมื่น",
            "กรมหลวง",
            "กรรมวิธี",
            "กรรมาธิการ",
            "กลวิธี",
            "กลอักษร",
            "กาลสมัย",
            "กาฬโรค",
            "กำเนิด",
            "กุลธิดา",
            "กุลสตรี",
            "เกษตรกรรม"
        };
    }

    public class Student {
        [Key]
        public int Id { set; get; }
        public string Name { set; get; }
        public string Profile1 { set; get; }
        public string Profile2 { set; get; }
        public string Profile3 { set; get; }
        public string ThaiText { set; get; } = Text.A[new Random().Next(Text.A.Length)];

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
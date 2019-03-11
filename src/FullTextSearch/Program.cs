using System;
using FullTextSearch.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bogus;
using WaffleGenerator.Bogus;
using DynamicTables;

// https://www.npgsql.org/efcore/mapping/full-text-search.html

namespace FullTextSearch {
    class Program {

        static DbContextOptions CreateOptions() {
            var conn = "Host=localhost;User Id=postgres;Password=1234;Database=FullTextSearch";
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(conn);
            return builder.Options;
        }

        static bool Insert(int size) {
            var options = CreateOptions();
            var faker =
                new Faker<Student>()
                    .CustomInstantiator(f => new Student {
                        //Name = f.Name.FirstName(),
                        //Profile1 = f.Lorem.Paragraph(),
                        //Profile2 = f.Lorem.Paragraph(),
                        //Profile3 = f.Lorem.Paragraph()
                    })
                    .RuleFor(s => s.Name, f => f.Name.FirstName())
                    .RuleFor(s => s.Profile1, f => f.WaffleText(2))
                    .RuleFor(s => s.Profile2, f => f.WaffleText(2))
                    .RuleFor(s => s.Profile3, f => f.WaffleText(2))
                    .FinishWith((f, s) => {
                        // Console.WriteLine(s.Profile1);
                    });

            var count = 0;

            foreach (var item in Enumerable.Range(0, 10)) {
                var items = size;
                var data = Enumerable.Range(1, items).Select(x => faker.Generate());
                using (var context = new MyContext(options)) {
                    context.Database.EnsureCreated();
                    context.Students.AddRange(data);
                    context.SaveChanges();
                }
                Console.WriteLine(count += items);
            }

            return true;
        }

        static bool Count() {
            var options = CreateOptions();
            using (var context = new MyContext(options)) {
                var count = context.Students.Count();
                Console.WriteLine(count);
            }
            return true;
        }

        static bool Contains(string keywords) {
            var options = CreateOptions();
            using (var context = new MyContext(options)) {
                var data = context.Students
                    .Where(x =>
                        x.Profile1.Contains(keywords) ||
                        x.Profile2.Contains(keywords) ||
                        x.Profile3.Contains(keywords)
                    )
                    .Select(x => x.Name).ToList();
                Console.WriteLine(data.Count());
            }
            return true;
        }

        static bool Like(string keywords) {
            var options = CreateOptions();
            using (var context = new MyContext(options)) {
                var data = context.Students
                    .Where(x =>
                        EF.Functions.Like(x.Profile1, "%" + keywords + "%") ||
                        EF.Functions.Like(x.Profile2, "%" + keywords + "%") ||
                        EF.Functions.Like(x.Profile3, "%" + keywords + "%")
                    )
                    .Select(x => x.Name).ToList();
                Console.WriteLine(data.Count());
            }
            return true;
        }

        static bool FreeText(string keywords) {

            /*
            DROP INDEX fts_idx
            CREATE INDEX fts_idx ON public."Students" USING GIN (to_tsvector('english', "Profile1" || ' ' || "Profile2" || ' ' || "Profile3" ))
            */

            var options = CreateOptions();
            using (var context = new MyContext(options)) {
                var data = context.Students
                    .Where(x =>
                            EF.Functions
                                .ToTsVector("english", x.Profile1 + " " + x.Profile2 + " " + x.Profile3)
                                .Matches(keywords)
                    ).Select(x => x.Name).ToList();
                Console.WriteLine(data.Count);
            }
            return true;
        }

        static bool OrderBy() {
            var options = CreateOptions();
            using (var context = new MyContext(options)) {
                var rs = context.Students.Take(20).Select(x => new {
                    Name = x.Name,
                    Text = x.ThaiText
                }).OrderBy(x => x.Text);
                DynamicTable.From(rs).Write();
            }
            return true;
        }

        static bool Clear() {
            var options = CreateOptions();
            using (var context = new MyContext(options)) {
                context.Database.EnsureDeleted();
            }
            return true;
        }

        static void Main(string[] args) {
            if (args.Length == 0) return;

            var keywords = "object";

            var rs = args[0] switch
            {
                "--insert-1" => Insert(10),
                "--insert-2" => Insert(1000),
                "--clear" => Clear(),
                "--count" => Count(),
                "--like" => Like(keywords),
                "--contains" => Contains(keywords),
                "--freetext" => FreeText(keywords),
                "--order" => OrderBy(),
                _ => true
            };
        }
    }
}

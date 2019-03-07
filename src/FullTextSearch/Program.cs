using System;
using FullTextSearch.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bogus;
using WaffleGenerator.Bogus;

// https://www.npgsql.org/efcore/mapping/full-text-search.html

namespace FullTextSearch {
    class Program {

        static DbContextOptions CreateOptions() {
            var conn = "Host=localhost;User Id=postgres;Password=1234;Database=FullTextSearch";
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(conn);
            return builder.Options;
        }

        static void Main(string[] args) {
            var options = CreateOptions();

            if (args.Contains("--insert")) {
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
                    var items = 1000;
                    var data = Enumerable.Range(1, items).Select(x => faker.Generate());
                    using (var context = new MyContext(options)) {
                        context.Database.EnsureCreated();
                        context.Students.AddRange(data);
                        context.SaveChanges();
                    }
                    Console.WriteLine(count += items);
                }
            }

            if (args.Contains("--count")) {
                using (var context = new MyContext(options)) {
                    var count = context.Students.Count();
                    Console.WriteLine(count);
                }
            }

            var keywords = "object";

            if (args.Contains("--contains")) {
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
            }

            /*
            DROP INDEX fts_idx
            CREATE INDEX fts_idx ON public."Students" USING GIN (to_tsvector('english', "Profile1" || ' ' || "Profile2" || ' ' || "Profile3" ))
            */

            if (args.Contains("--freetext")) {
                using (var context = new MyContext(options)) {
                    var data = context.Students
                        .Where(x =>
                                EF.Functions
                                    .ToTsVector("english", x.Profile1 + " " + x.Profile2 + " " + x.Profile3)
                                    .Matches(keywords)
                        ).Select(x => x.Name).ToList();
                    Console.WriteLine(data.Count);
                }
            }
        }
    }
}

using ColorWebsite.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace ColorWebsite.Data
{

    public class ApplicationDbContextSeed
    {
        public async Task SeedAsync(ApplicationDBContext context, IServiceProvider services, int? retry = 0)
        {

            int retryForAvaiability = retry.Value;
            try
            {

                if (!context.Colors.Any())
                {
                    Console.WriteLine("No Colors");

                    context.Colors.AddRange(
                         new DemoColor { Name = "Orange" },
                         new DemoColor { Name = "Blue" },
                         new DemoColor { Name = "DarkGray" }
                       );

                    await context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;
                    await SeedAsync(context, services, retryForAvaiability);
                }
            }

        }

    }
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> option)
            : base(option)
        {
        }

        public DbSet<DemoColor> Colors { get; set; }
    }


}

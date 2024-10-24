using System.Data.Entity;
using System.Linq;
using FurniflexBE.Models;

namespace FurniflexBE.Context
{
    public class AppDbInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            // Seed roles if they do not exist
            if (!context.roles.Any())
            {
                context.roles.Add(new Role { Name = "admin" });
                context.roles.Add(new Role { Name = "customer" });
            }

            // Save changes
            context.SaveChanges();

            base.Seed(context);
        }
    }
}

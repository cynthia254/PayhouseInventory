using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.INFRASTRUCTURE.DataContext
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DragonFlyContext>
    {
        public DragonFlyContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DragonFlyContext>();

            // Read configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DevConnectiions");

            optionsBuilder.UseSqlServer(connectionString);

            return new DragonFlyContext(optionsBuilder.Options);
        }
    }
}

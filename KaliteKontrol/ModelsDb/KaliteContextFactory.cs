using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace KaliteKontrol.ModelsDb
{
    public class KaliteContextFactory : IDesignTimeDbContextFactory<KaliteContext>
    {
        public KaliteContext CreateDbContext(string[] args)
        {
            // appsettings.json'dan connection string oku
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<KaliteContext>();
            var conMsSqlServer = configuration.GetConnectionString("conMsSql");
            optionsBuilder.UseSqlServer(conMsSqlServer);

            return new KaliteContext(optionsBuilder.Options);
        }
    }
}

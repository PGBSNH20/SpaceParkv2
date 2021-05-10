using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpacePark_API.DataAccess;
using System.Linq;
using System.Net.Http;

namespace SpaceParkTests
{
    public class FakeWebHost<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        
        private readonly string _dbName = "MockDB";
        public IConfiguration Configuration { get; private set; }

        public new HttpClient CreateClient()
        {
            return base.CreateClient();
        }
        private StarwarsContext _inMemoryContext;
        public StarwarsContext InMemoryContext
        {
            get
            {
                if (_inMemoryContext == null)
                {
                    var options = new DbContextOptionsBuilder<StarwarsContext>()
                             .UseInMemoryDatabase(databaseName: _dbName)
                             .Options;
                    _inMemoryContext = new StarwarsContext(options);
                }
                return _inMemoryContext;
            }
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<StarwarsContext>));

                services.Remove(descriptor);
                
                services.AddDbContext<StarwarsContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName: _dbName);
                });
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var dbContext = scopedServices.GetRequiredService<StarwarsContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<FakeWebHost<TStartup>>>();

                    dbContext.Database.EnsureCreated();
                }
            });
        }
    }
}

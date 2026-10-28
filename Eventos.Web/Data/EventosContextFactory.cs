using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Eventos.Web.Data;
public class EventosContextFactory : IDesignTimeDbContextFactory<EventosContext>
{
    public EventosContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
        return new EventosContext(new DbContextOptionsBuilder<EventosContext>().UseSqlServer(config.GetConnectionString("EventosDB")).Options);
    }
}

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SO_tags;
using SO_tags.Models;


internal class TagsApiFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    base.ConfigureWebHost(builder);
    builder.ConfigureTestServices(async services =>
    {
      services.RemoveAll(typeof(LocalTagsContext));
      services.AddDbContext<LocalTagsContext>(options =>
          options.UseSqlite("Data Source=tags_tests.dat"));

      var dbContext = CreateDbContext(services);
      await dbContext.Database.EnsureDeletedAsync();
      await dbContext.Database.EnsureCreatedAsync();
      dbContext.Add(new Tag{
        Name = "sampleTag",
        Count = 155555
      });
      dbContext.SaveChanges();
    });
  }

    private static LocalTagsContext CreateDbContext(IServiceCollection services)
    {
        var servicesProvider = services.BuildServiceProvider();
        var scope = servicesProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<LocalTagsContext>();
    }
}
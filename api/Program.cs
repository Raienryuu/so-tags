using Microsoft.EntityFrameworkCore;
using SO_tags.Options;
using SO_tags.Providers;

namespace SO_tags;

public class Program
{
  public static void Main(string[] args)
  {
	var builder = WebApplication.CreateBuilder(args);

	builder.Services.AddAuthorization();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();
	builder.Services.AddControllers();
	builder.Services.AddScoped<IRemoteTagsProvider, SoTagsProvider>();
	builder.Services.AddLogging();

	builder.Services.AddDbContext<LocalTagsContext>(o =>
	  o.UseSqlite(builder.Configuration["ConnectionStrings:LocalFileDB"]));
	string test = builder.Configuration["ConnectionStrings:LocalFileDB"];

	builder.Services.Configure<ApiOptions>(
	  builder.Configuration.GetSection("ApiOptions:StackExchange"));

	var db = GetDatabaseContext(builder.Services);
	db.Database.EnsureDeletedAsync();
	db.Database.EnsureCreatedAsync();

	var app = builder.Build();

	// if (app.Environment.IsDevelopment())
	// {
	app.UseSwagger();
	app.UseSwaggerUI(o =>
	{
	  o.SwaggerEndpoint("/swagger/openapi.json", "v1");
	  o.RoutePrefix = string.Empty;
	});
	// }

	app.UseHttpsRedirection();

	app.UseStaticFiles();

	app.MapControllers().WithOpenApi();

	app.Run();
  }

  private static LocalTagsContext GetDatabaseContext(
	IServiceCollection services)
  {
	var servicesProvider = services.BuildServiceProvider();
	var scope = servicesProvider.CreateScope();
	return scope.ServiceProvider.GetRequiredService<LocalTagsContext>();
  }
}
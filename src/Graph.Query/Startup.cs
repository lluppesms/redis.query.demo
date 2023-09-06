[assembly: FunctionsStartup(typeof(Graph.Query.Startup))]
namespace Graph.Query;

/// <summary>
/// Graph.Query.Startup
/// </summary>
public class Startup : FunctionsStartup
{
	/// <summary>
	/// Configuration Intializer
	/// </summary>
	public override void Configure(IFunctionsHostBuilder builder)
	{
		// ----- Configure Services -----------------------------------------------------------------------
		var settings = new Settings();
		var redisConnection = ConnectionMultiplexer.Connect(settings.RedisConnectionString);

		builder.Services.AddSingleton(settings);
		builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
		builder.Services.AddSingleton(new RedisHelper(settings, redisConnection));
	}

	/// <summary>
	/// Configuration Intializer
	/// </summary>
	public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
	{
		builder.ConfigurationBuilder
		   .SetBasePath(Environment.CurrentDirectory)
		   .AddJsonFile("local.settings.json", true)
		   .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
		   .AddEnvironmentVariables()
		   .Build();
	}
}


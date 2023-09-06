namespace Graph.Query.Models;

public class Settings
{
	/// <summary>
	/// Redis Name
	/// </summary>
	public string RedisCacheName { get; set; }

	/// <summary>
	/// Redis Key
	/// </summary>
	public string RedisCacheKey { get; set; }

	/// <summary>
	/// Redis Connection String
	/// </summary>
	public string RedisConnectionString { get; set; }

	/// <summary>
	/// Constructor
	/// </summary>
	public Settings()
	{
		LoadSecrets();
	}

	/// <summary>
	/// Constructor
	/// </summary>
	public Settings(string redisCacheName, string redisCacheKey, string redisConnectionString)
	{
		RedisCacheName = redisCacheName;
		RedisCacheKey = redisCacheKey;
		RedisConnectionString = redisConnectionString;
	}

	/// <summary>
	/// Load secrets from Environment
	/// </summary>
	public void LoadSecrets()
	{
		if (string.IsNullOrEmpty(RedisCacheName))
		{
			RedisCacheName = Environment.GetEnvironmentVariable("RedisCacheName");
			RedisCacheKey = Environment.GetEnvironmentVariable("RedisCacheKey");
			RedisConnectionString = Environment.GetEnvironmentVariable("RedisConnectionString");
		}
	}

	/// <summary>
	/// Echo out values
	/// </summary>
	public void DisplayValues(string dataSource)
	{
		MyLogger.LogInfo($"Settings.RedisCacheName: {RedisCacheName}", dataSource);
		MyLogger.LogInfo($"Settings.RedisCacheKey: {RedisCacheKey[0..4]}", dataSource);
		MyLogger.LogInfo($"Settings.RedisConnectionString: {Common.GetSanitizedConnectionString(RedisConnectionString)}", dataSource);
	}
}

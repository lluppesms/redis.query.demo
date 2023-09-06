using Microsoft.Identity.Client;
using System.Diagnostics;

namespace Graph.Query.DataRepository;

// See https://stackexchange.github.io/StackExchange.Redis/Basics.html
// Note 1: The object returned from GetDatabase is a cheap pass-thru object, and does not need to be stored (even though I'm doing that...)
// Note 2: Database name can be optionally specified in the call to GetDatabase -> IDatabase db = redis.GetDatabase(databaseNumber, asyncState);
// Note 3: For all commands, see https://redis.io/commands/ --> see the ClusterReset method for a broken example of how to use then (or not!)

// Load scenario = dataset of 10,000 nodes
// Generate role assignment for 3 users
//   - pick 10 permissions for one user out of this 10,000 nodes
//   - pick 100 permissions for one user out of this 10,000 nodes
//   - pick 1000 permissions for one user out of this 10,000 nodes

/// <summary>
/// Redis Helper Class
/// </summary>
public class RedisHelper
{
	#region Variables
	private readonly Settings settings;
	private IConnectionMultiplexer connectionMultiplexer;
	private IDatabase db;
	private IServer redisServer;
	#endregion

	#region Initialization
	/// <summary>
	/// Initializer
	/// </summary>
	public RedisHelper()
	{
		settings = new Settings();
		connectionMultiplexer = ConnectionMultiplexer.Connect(settings.RedisConnectionString);
		db = connectionMultiplexer.GetDatabase();
		redisServer = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
	}
	/// <summary>
	/// Initializer
	/// </summary>
	public RedisHelper(Settings diSettings, IConnectionMultiplexer connectionMultiplexer)
	{
		settings = diSettings;
		connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
		db = connectionMultiplexer.GetDatabase();
		redisServer = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
	}
	/// <summary>
	/// Rehydrate Redis Connections if they are null
	/// </summary>
	private void RehydrateRedisConnections()
	{
		if (!string.IsNullOrEmpty(settings.RedisConnectionString))
		{
			connectionMultiplexer = ConnectionMultiplexer.Connect(settings.RedisConnectionString);
			db = connectionMultiplexer.GetDatabase();
			redisServer = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
		}
	}
	#endregion

	/// <summary>
	/// Load a simple sample dataset
	/// </summary>
	public async Task<ResultMessage> LoadSampleData(string userId, string userId2 = "", string callingFunction = "LoadSampleData")
	{
		var user1Name = $"UNP:{userId}";
		var user1Permissions = new RedisValue[] { "NSG:ACCT1", "NSG:ACCT2" };
		var user2Name = $"UNP:{userId2}";
		var user2Permissions = new RedisValue[] { "NSG:ACCT2", "NSG:ACCT3", "NSG:SITE23", "NSG:SITE77", "ASSET104" };

		var node1Name = "NSG:ACCT1";
		var node1SubGraph = new RedisValue[] { "NSG:SITE1", "NSG:SITE2" };
		var node2Name = "NSG:ACCT2";
		var node2SubGraph = new RedisValue[] { "NSG:SITE20", "NSG:SITE21" };
		var node3Name = "NSG:SITE1";
		var node3SubGraph = new RedisValue[] { "ASSET101", "ASSET102", "ASSET103" };
		var node4Name = "NSG:SITE2";
		var node4SubGraph = new RedisValue[] { "ASSET201", "ASSET202", "ASSET203" };
		var node5Name = "NSG:SITE3";
		var node5SubGraph = new RedisValue[] { "ASSET301", "ASSET302", "ASSET303" };

		var totalTimer = new Stopwatch();
		totalTimer.Start();

		string status;
		var output = string.Empty;
		try
		{
			(var success, var recordsAdded, var message) = await InsertValues(user1Name, user1Permissions);
			output += message;

			if (!string.IsNullOrEmpty(user2Name))
			{
				(success, recordsAdded, message) = await InsertValues(user2Name, user2Permissions);
				output += message;
			}

			(success, recordsAdded, message) = await InsertValues(node1Name, node1SubGraph);
			output += message;

			(success, recordsAdded, message) = await InsertValues(node2Name, node2SubGraph);
			output += message;

			(success, recordsAdded, message) = await InsertValues(node3Name, node3SubGraph);
			output += message;

			(success, recordsAdded, message) = await InsertValues(node4Name, node4SubGraph);
			output += message;

			(success, recordsAdded, message) = await InsertValues(node5Name, node5SubGraph);
			output += message;

			status = $"  Insert records complete in {Common.ElapsedSeconds(totalTimer)}!";
			Console.WriteLine(status);
			output += status;
			return new ResultMessage(callingFunction, output);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage($"{callingFunction} LoadSampleData: {output} ", ex);
			return error;
		}
	}

	/// <summary>
	/// Load a large sample scenario with 10,000 accounts, and users with 10, 100, and 1000 account access
	/// </summary>
	public async Task<ResultMessage> LoadLargeScenarioData(int recordCount = 2500, string callingFunction = "LoadScenarioData")
	{
		var accountListKey = Constants.RedisKeyValue.ScenarioAccounts;
		var accounts = new List<RedisValue>();
		var accountIds = new List<int>();
		RedisValue[] accountList;

		var siteListKey = Constants.RedisKeyValue.ScenarioSites;
		var sites = new List<RedisValue>();
		RedisValue[] siteList;

		var userListKey = Constants.RedisKeyValue.ScenarioUsers;
		var users = new List<RedisValue>();
		RedisValue[] userList;

		var rnd = new Random(Guid.NewGuid().GetHashCode());
		var output = string.Empty;
		bool success;
		int recordsAdded;
		string message;
		var totalTimer = new Stopwatch();
		totalTimer.Start();

		try
		{
			// Generate some random accounts
			for (int i = 0; i < recordCount; i++)
			{
				var thisValue = rnd.Next(99900) + 1000;
				var thisAccountName = $"NSG:ACCT{thisValue}";
				accounts.Add(thisAccountName);
				accountIds.Add(thisValue);
				var subGraph = new RedisValue[] { $"SITE{thisValue}", $"SITE{thisValue + 1}", $"ASSET{thisValue}" };
				(success, recordsAdded, message) = await InsertValues(thisAccountName, subGraph);
			}
			accountList = accounts.ToArray();
			(success, recordsAdded, message) = await InsertValues(accountListKey, accountList);
			Console.WriteLine(message);
			output += message;

			// Generate some sites based on the accounts...
			foreach (var accountId in accountIds)
			{
				var thisSiteName = $"NSG:SITE{accountId}";
				sites.Add(thisSiteName);
				var assetId = accountId * 10;
				var subGraph = new RedisValue[] { $"ASSET{assetId + 1}", $"ASSET{assetId + 2}" , $"ASSET{assetId + 3}" };
				(success, recordsAdded, message) = await InsertValues(thisSiteName, subGraph);
				//Console.WriteLine(message);
				output += message;

				thisSiteName = $"NSG:SITE{accountId + 1}";
				sites.Add(thisSiteName);
				assetId = (accountId + 1) * 10;
				subGraph = new RedisValue[] { $"ASSET{assetId + 1}", $"ASSET{assetId + 2}", $"ASSET{assetId + 3}" };
				(success, recordsAdded, message) = await InsertValues(thisSiteName, subGraph);
				//Console.WriteLine(message);
				output += message;
			}
			siteList = sites.ToArray();
			(success, recordsAdded, message) = await InsertValues(siteListKey, siteList);
			Console.WriteLine(message);
			output += message;

			// Generate a user with access up to 10 of the accounts (excludes duplicates...)
			var userName = "UNP:10";
			users.Add(userName);
			var userRights = new List<RedisValue>();
			for (int i = 0; i < 10; i++)
			{
				var ndx = rnd.Next(accounts.Count);
				var thisAccount = accounts[ndx];
				userRights.Add(thisAccount);
			}
			RedisValue[] userPermissions = userRights.ToArray();
			(success, recordsAdded, message) = await InsertValues(userName, userPermissions);
			Console.WriteLine(message);
			output += message;

			// Generate a user with access up to 100 of the accounts (excludes duplicates...)
			userName = "UNP:100";
			users.Add(userName);
			userRights = new List<RedisValue>();
			var maxUser100Count = recordCount < 100 ? recordCount : 100;
			for (int i = 0; i < maxUser100Count; i++)
			{
				var ndx = rnd.Next(accounts.Count);
				var thisAccount = accounts[ndx];
				userRights.Add(thisAccount);
			}
			userPermissions = userRights.ToArray();
			(success, recordsAdded, message) = await InsertValues(userName, userPermissions);
			Console.WriteLine(message);
			output += message;

			// Generate a user with access up to 1000 of the accounts (excludes duplicates...)
			userName = "UNP:1000";
			users.Add(userName);
			userRights = new List<RedisValue>();
			var maxUser1000Count = recordCount < 1000 ? recordCount : 1000;
			for (int i = 0; i < maxUser1000Count; i++)
			{
				var ndx = rnd.Next(accounts.Count);
				var thisAccount = accounts[ndx];
				userRights.Add(thisAccount);
			}
			userPermissions = userRights.ToArray();
			(success, recordsAdded, message) = await InsertValues(userName, userPermissions);
			Console.WriteLine(message);
			output += message;


			// Insert the generic list of users for this scenario
			userList = users.ToArray();
			(success, recordsAdded, message) = await InsertValues(userListKey, userList);
			Console.WriteLine(message);
			output += message;

			message = $"  Scenario created in {Common.ElapsedSeconds(totalTimer)}!";
			Console.WriteLine(message);
			output += message;
			return new ResultMessage(callingFunction, output);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage($"{callingFunction} LoadSampleData: {output} ", ex);
			return error;
		}
	}

	/// <summary>
	/// Return list of accounts for this scenario
	/// </summary>
	public async Task<(bool, RedisValue[], string)> ListScenarioAccounts()
	{
		return await ListValuesForOneKey(Constants.RedisKeyValue.ScenarioAccounts);
	}

	/// <summary>
	/// Return list of users for this scenario
	/// </summary>
	public async Task<(bool, RedisValue[], string)> ListScenarioUsers()
	{
		return await ListValuesForOneKey(Constants.RedisKeyValue.ScenarioUsers);
	}

	/// <summary>
	/// Return list of accounts for one user in the scenario
	/// </summary>
	/// <param name="id">User Id</param>
	/// <returns>List of accounts</returns>
	public async Task<(bool, List<RedisKey>, RedisValue[], string)> ListScenarioUserRights(string id)
	{
		return await UnionKeyQuery(id);
	}

	/// <summary>
	/// Return filtered list of accounts for one user in the scenario
	/// </summary>
	/// <param name="id">User Id</param>
	/// <param name="startsWith">Permission starts with</param>
	/// <returns>List of accounts</returns>
	public async Task<(bool, List<RedisKey>, RedisValue[], string)> ListScenarioFilteredUserRights(string id, string startsWith)
	{
		(bool success, List<RedisKey> keys, RedisValue[] values, string message) = await UnionKeyQuery(id);
		var filteredValues = values.Where(x => x.StartsWith(startsWith)).ToArray();
		Array.Sort(filteredValues);
		var stringResult = string.Join(", ", Array.ConvertAll(filteredValues, item => (string)item));
		return (success, keys, filteredValues, stringResult);
	}

	/// <summary>
	/// Add records
	/// </summary>
	public async Task<(bool, int, string)> InsertValues(string keyValue, RedisValue[] values)
	{
		var areaTimer = new Stopwatch();
		areaTimer.Start();
		try
		{
			var recordsAdded = 0;
			foreach (var value in values)
			{
				await db.SetAddAsync(keyValue, value);
				recordsAdded++;
			}
			var msg = $"Added {recordsAdded} records for {keyValue} in {Common.ElapsedSeconds(areaTimer)}!";
			Console.WriteLine(msg);
			return (true, recordsAdded, msg);
		}
		catch (Exception ex)
		{
			var errorMsg = MyLogger.LogException($"Error in RedisHelper.InsertValues:", ex);
			return (false, 0, errorMsg);
		}
	}

	/// <summary>
	/// Union records
	/// </summary>
	public async Task<(bool, List<RedisKey>, RedisValue[], string)> UnionKeyQuery(string keyValue)
	{
		RedisValue[] result = Array.Empty<RedisValue>();
		var stringResult = string.Empty;
		try
		{
			var members = await db.SetMembersAsync(keyValue);
			var keys = new List<RedisKey>();
			foreach (var member in members.Select(x => x.ToString()))
			{
				keys.Add(member);
			}
			// Perform the SUNION operation
			if (keys.Count > 0)
			{
				result = await db.SetCombineAsync(SetOperation.Union, keys.ToArray());
				Array.Sort(result);
				stringResult = string.Join(", ", Array.ConvertAll(result, item => (string)item));
			}
			return (true, keys, result, stringResult);
		}
		catch (Exception ex)
		{
			var errorMsg = MyLogger.LogException($"Error in RedisHelper.UnionKeyQuery:", ex);
			return (false, null, null, errorMsg);
		}
	}

	/// <summary>
	/// (Incomplete!) List all values for one key
	/// </summary>
	public async Task<(bool, RedisKey[], string)> ListAllKeys()
	{
		_ = await Task.FromResult(true); // async placeholder...
		try
		{
			var allKeys = redisServer.Keys(pattern: "*").ToArray();
			var result = string.Empty;
			if (allKeys.Length > 0)
			{
				result = string.Join(", ", Array.ConvertAll(allKeys, item => (string)item));
			}
			return (true, allKeys, result);
		}
		catch (Exception ex)
		{
			var errorMsg = MyLogger.LogException($"Error in RedisHelper.ListAllKeys:", ex);
			return (false, null, errorMsg);
		}
	}

	/// <summary>
	/// List all values for one key
	/// </summary>
	public async Task<(bool, RedisValue[], string)> ListValuesForOneKey(string keyValue)
	{
		var stringResult = string.Empty;
		try
		{
			var members = await db.SetMembersAsync(keyValue);
			if (members.Length > 0)
			{
				stringResult = string.Join(", ", Array.ConvertAll(members, item => (string)item));
			}
			return (true, members, stringResult);
		}
		catch (Exception ex)
		{
			var errorMsg = MyLogger.LogException($"Error in RedisHelper.ListForKey:", ex);
			return (false, null, errorMsg);
		}
	}

	/// <summary>
	/// Delete records for one key
	/// </summary>
	public async Task<(bool, string)> DeleteOneKey(string keyValue)
	{
		try
		{
			var success = await db.KeyDeleteAsync(keyValue);
			var msg = $"Deleted records for {keyValue}.";
			return (true, msg);
		}
		catch (Exception ex)
		{
			var errorMsg = MyLogger.LogException($"Error in RedisHelper.Delete:", ex);
			return (false, errorMsg);
		}
	}

	/// <summary>
	/// Deletes all values in the database
	/// </summary>
	public async Task<(bool, int, string)> DeleteAllKeys()
	{
		try
		{
			var allKeys = redisServer.Keys(pattern: "*");
			var recordsDeleted = 0;
			foreach (var key in allKeys)
			{
				var success = await db.KeyDeleteAsync(key);
				recordsDeleted++;
			}
			var result = $"Deleted {recordsDeleted} records!";
			return (true, recordsDeleted, result);
		}
		catch (Exception ex)
		{
			var errorMsg = MyLogger.LogException($"Error in RedisHelper.DeleteAllKeys:", ex);
			return (false, 0, errorMsg);
		}
	}

	///// <summary>
	///// Cluster Reset Command -- not working!!!
	///// </summary>
	//public async Task<(bool, string)> ClusterReset()
	//{
	//	// For all commands, see https://redis.io/commands/
	//	// Thought we could use those commands but the format is wrong...
	//	// It fails with this error:
	//	//   ERR unknown command \u0060CLUSTER RESET SOFT\u0060, with args beginning with:
	//	try
	//	{
	//		var redisResult = await db.ExecuteAsync("CLUSTER RESET SOFT");
	//		var msg = $"Cluster Reset Result: {redisResult}";
	//		return (true, msg);
	//	}
	//	catch (Exception ex)
	//	{
	//		var errorMsg = MyLogger.LogException($"Error in RedisHelper.ClusterReset:", ex);
	//		return (false, errorMsg);
	//	}
	//}
}

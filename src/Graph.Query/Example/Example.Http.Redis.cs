//namespace Graph.Query;

//// TODO: add a delete data option here...

//public class RedisQueryExample
//{
//	#region Variables
//	private readonly string LogDataSource = Constants.DataSource.Interaction.Redis;
//	private readonly IConnectionMultiplexer _connectionMultiplexer;

//	private readonly string adminName = "rutzsco";
//	private readonly RedisValue[] adminPermissions = new RedisValue[] { "ACCNT1", "ACCNT2", "SITE1", "SITE2", "ASSET001", "ASSET002" };
//	private readonly string account1Name = "ACCNT1";
//	private readonly RedisValue[] account1SubGraph = new RedisValue[] { "ACCNT1", "SITE1", "SITE2", "ASSET001", "ASSET002" };
//	private readonly string account2Name = "ACCNT2";
//	private readonly RedisValue[] account2SubGraph = new RedisValue[] { "ACCNT2", "SITE20", "SITE21", "ASSET201", "ASSET202" };
//	#endregion

//	public RedisQueryExample(ILogger<Interaction_Trigger> logger, IConnectionMultiplexer connectionMultiplexer)
//	{
//		MyLogger.InitializeLogger(logger);
//		_connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
//	}

//	[FunctionName("ExampleLoad")]
//	public async Task<IActionResult> ExampleLoadEndpoint([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext executionContext)
//	{
//		_ = await Task.FromResult(true); // async placeholder

//		try
//		{
//			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName}", LogDataSource);

//			var db = _connectionMultiplexer.GetDatabase();
//			var recordsAdded = 0;
//			var msg = $"{executionContext.FunctionName}";
//			foreach (var value in adminPermissions)
//			{
//				db.SetAdd(adminName, value);
//				recordsAdded++;
//			}
//			msg += $"  Added {recordsAdded} records for {adminName}.";
//			recordsAdded = 0;
//			foreach (var value in account1SubGraph)
//			{
//				db.SetAdd(account1Name, value);
//				recordsAdded++;
//			}
//			msg += $"  Added {recordsAdded} records for {account1Name}.";
//			recordsAdded = 0;
//			foreach (var value in account2SubGraph)
//			{
//				db.SetAdd(account2Name, value);
//				recordsAdded++;
//			}
//			msg += $"  Added {recordsAdded} records for {account2Name}.";
//			recordsAdded = 0;
//			msg += $"  Action complete!";
//			MyLogger.LogObject(msg, LogDataSource);
//			return new OkObjectResult(msg);
//		}
//		catch (Exception ex)
//		{
//			var errorMsg = MyLogger.LogException($"Error in {executionContext.FunctionName}:", ex);
//			return new BadRequestObjectResult(errorMsg);
//		}
//	}

//	[FunctionName("ExampleUnion")]
//	public async Task<IActionResult> ExampleUnionEndpoint([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext executionContext)
//	{
//		_ = await Task.FromResult(true); // async placeholder
//		try
//		{
//			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName}", LogDataSource);

//			var db = _connectionMultiplexer.GetDatabase();

//			var members = db.SetMembers(adminName);
//			var readCount = 0;
//			var keys = new List<RedisKey>();
//			foreach (var member in members.Select(x => x.ToString()))
//			{
//				keys.Add(member);
//				readCount++;
//			}

//			// Perform the SUNION operation
//			var result = db.SetCombine(SetOperation.Union, keys.ToArray());
//			var recordCount = result.Length;

//			// Convert RedisValue[] to string for easier use
//			var stringResult = string.Join(", ", Array.ConvertAll(result, item => (string)item));
//			var msg = $"{executionContext.FunctionName} User: {adminName}  RecordsRead: {readCount}  SUnionCount: {recordCount}  Result: {stringResult}";
//			MyLogger.LogObject(msg, LogDataSource);
//			return new OkObjectResult(msg);
//		}
//		catch (Exception ex)
//		{
//			var errorMsg = MyLogger.LogException($"Error in {executionContext.FunctionName}:", ex);
//			return new BadRequestObjectResult(errorMsg);
//		}
//	}

//	[FunctionName("ExampleGetSet")]
//	public async Task<IActionResult> ExampleGetSetEndpoint([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log, ExecutionContext executionContext)
//	{
//		_ = await Task.FromResult(true); // async placeholder
//		try
//		{
//			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName}", LogDataSource);

//			var db = _connectionMultiplexer.GetDatabase();

//			var members = db.SetMembers(adminName);

//			// Convert RedisValue[] to string for easier use
//			var stringResult = string.Join(", ", Array.ConvertAll(members, item => (string)item));
//			var msg = $"{executionContext.FunctionName} User: {adminName}  MembersCount: {members.Length}  Result: {stringResult}";
//			MyLogger.LogObject(msg, LogDataSource);
//			return new OkObjectResult(msg);
//		}
//		catch (Exception ex)
//		{
//			var errorMsg = MyLogger.LogException($"Error in {executionContext.FunctionName}:", ex);
//			return new BadRequestObjectResult(errorMsg);
//		}
//	}
//}
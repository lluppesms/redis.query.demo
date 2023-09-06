namespace Graph.Query;

public class Http_Trigger_RedisQuery
{
	#region Variables
	private readonly string LogDataSource = Constants.DataSource.Triggers.RedisQuery;
	private readonly RedisHelper redisHelper = null;
	#endregion

	#region Initialization
	public Http_Trigger_RedisQuery(ILogger<Http_Trigger_Hello> logger, RedisHelper diRedisHelper)
	{
		MyLogger.InitializeLogger(logger);
		redisHelper = diRedisHelper;
	}
	#endregion

	/// <summary>
	/// Scenario Data Load
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "scenarioLoad", tags: new[] { "name" }, Summary = "Loads data for a scenario", Description = "Loads data for a scenario", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiParameter(name: "recordCount", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Number of account records to generate", Description = "Number of account records to generate", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(ResultMessage), Summary = "Structured response", Description = "This returns a structured response")]
	#endregion
	[FunctionName("ScenarioLoad")]
	public async Task<IActionResult> ScenarioLoad([HttpTrigger(AuthorizationLevel.Function, "get", Route = "scenario/load/{recordCount}")] HttpRequest req, int recordCount, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName}", LogDataSource);

			var resultMessage = await redisHelper.LoadLargeScenarioData(recordCount, executionContext.FunctionName);

			var obj = MyLogger.LogResultMessage(resultMessage, LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}

	/// <summary>
	/// List Accounts for the current scenario
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "scenarioAccounts", tags: new[] { "name" }, Summary = "Returns a lists of the accounts for the current scenario", Description = "Returns a lists of the accounts for the current scenario", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(string), Summary = "Structured response", Description = "This returns a structured response")]
	#endregion
	[FunctionName("ScenarioAccounts")]
	public async Task<IActionResult> ScenarioAccounts([HttpTrigger(AuthorizationLevel.Function, "get", Route = "scenario/accounts")] HttpRequest req, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName}.", LogDataSource);

			(var success, var valueList, var valuesString) = await redisHelper.ListScenarioAccounts();

			var obj = MyLogger.LogResultMessage(new ResultMessage(executionContext.FunctionName, valueList.Length, valuesString, timer), LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}

	/// <summary>
	/// List Users for the current scenario
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "scenarioUsers", tags: new[] { "name" }, Summary = "Returns a lists of the users for the current scenario", Description = "Returns a lists of the users for the current scenario", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(string), Summary = "Structured response", Description = "This returns a structured response")]
	#endregion
	[FunctionName("ScenarioUsers")]
	public async Task<IActionResult> ScenarioUsers([HttpTrigger(AuthorizationLevel.Function, "get", Route = "scenario/users")] HttpRequest req, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName}.", LogDataSource);

			(var success, var valueList, var valuesString) = await redisHelper.ListScenarioUsers();

			var obj = MyLogger.LogResultMessage(new ResultMessage(executionContext.FunctionName, valueList.Length, valuesString, timer), LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}

	/// <summary>
	/// List User Rights for one user in the current scenario
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "scenarioUserRights", tags: new[] { "name" }, Summary = "List User Rights for one user in the current scenario", Description = "List User Rights for one user in the current scenario", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "The userId being queried", Description = "The userId being queried", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(string), Summary = "Structured response", Description = "This returns a structured response")]
	#endregion
	[FunctionName("ScenarioUserRights")]
	public async Task<IActionResult> ScenarioUserRights([HttpTrigger(AuthorizationLevel.Function, "get", Route = "scenario/user/{id}")] HttpRequest req, string id, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName} with id: {id}.", LogDataSource);

			(var success, var keyList, var valueList, var valuesString) = await redisHelper.ListScenarioUserRights(id);

			var obj = MyLogger.LogResultMessage(new ResultMessage(executionContext.FunctionName, id, keyList.Count, valueList.Length, valuesString, timer), LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}

	/// <summary>
	/// List Filtered User Rights for one user in the current scenario
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "scenarioUserFilteredRights", tags: new[] { "name" }, Summary = "List Filtered User Rights for one user in the current scenario", Description = "List Filtered User Rights for one user in the current scenario", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "The userId being queried", Description = "The userId being queried", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiParameter(name: "filter", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Limit results to this type of record", Description = "Suggested values: SITE, ASSET, NSG:ACCT", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(string), Summary = "Structured response", Description = "This returns a structured response")]
	#endregion
	[FunctionName("ScenarioUserFilteredRights")]
	public async Task<IActionResult> ScenarioUserFilteredRights([HttpTrigger(AuthorizationLevel.Function, "get", Route = "scenario/user/{filter}/{id}")] HttpRequest req, string filter, string id, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName} with id: {id}.", LogDataSource);

			(var success, var keyList, var valueList, var valuesString) = await redisHelper.ListScenarioFilteredUserRights(id, filter);

			var obj = MyLogger.LogResultMessage(new ResultMessage(executionContext.FunctionName, id, keyList.Count, valueList.Length, valuesString, timer), LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}

	/// <summary>
	/// Generic Load Data
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "loadSampleData", tags: new[] { "name" }, Summary = "Loads the sample data", Description = "This operation loads the initial sample data into Redis.", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "The userId being granted rights", Description = "The userId being granted rights", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
	#endregion
	[FunctionName("LoadSampleData")]
	public async Task<IActionResult> LoadSampleData([HttpTrigger(AuthorizationLevel.Function, "get", Route = "load/{id}")] HttpRequest req, string id, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName} with id: {id}", LogDataSource);

			var resultMessage = await redisHelper.LoadSampleData(id, executionContext.FunctionName);

			var obj = MyLogger.LogResultMessage(resultMessage, LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}

	/// <summary>
	/// Generic Union Query
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "union", tags: new[] { "name" }, Summary = "Runs a union query over the data", Description = "Runs a union query over the data", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "The userId being queried", Description = "The userId being queried", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(ResultMessage), Summary = "Structured response", Description = "This returns a structured response")]
	#endregion
	[FunctionName("UnionQuery")]
	public async Task<IActionResult> UnionQuery([HttpTrigger(AuthorizationLevel.Function, "get", Route = "union/{id}")] HttpRequest req, string id, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName} with id: {id}", LogDataSource);

			(var success, var keyList, var valueList, var valuesString) = await redisHelper.UnionKeyQuery(id);

			var obj = MyLogger.LogResultMessage(new ResultMessage(executionContext.FunctionName, id, keyList.Count, valueList.Length, valuesString, timer), LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}

	/// <summary>
	/// List all values in the database
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "listAllKeys", tags: new[] { "name" }, Summary = "Gets all values in the database", Description = "Gets all values in the database", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "The userId being queried", Description = "The userId being queried", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(ResultMessage), Summary = "Structured response", Description = "This returns a structured response")]
	#endregion
	[FunctionName("ListAllKeys")]
	public async Task<IActionResult> ListAllKeys([HttpTrigger(AuthorizationLevel.Function, "get", Route = "listall")] HttpRequest req, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName}", LogDataSource);

			(var success, var valueList, var valuesString) = await redisHelper.ListAllKeys();

			var obj = MyLogger.LogResultMessage(new ResultMessage(executionContext.FunctionName, valueList.Length, valuesString, timer), LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}

	/// <summary>
	/// List Values for one key
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "list", tags: new[] { "name" }, Summary = "Gets values for one key", Description = "Gets values for one key", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "The userId being queried", Description = "The userId being queried", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(ResultMessage), Summary = "Structured response", Description = "This returns a structured response")]
	#endregion
	[FunctionName("ListValues")]
	public async Task<IActionResult> ListValues([HttpTrigger(AuthorizationLevel.Function, "get", Route = "list/{id}")] HttpRequest req, string id, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName} with id: {id}", LogDataSource);

			(var success, var valueList, var valuesString) = await redisHelper.ListValuesForOneKey(id);

			var obj = MyLogger.LogResultMessage(new ResultMessage(executionContext.FunctionName, id, valueList.Length, valuesString, timer), LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}

	/// <summary>
	/// Delete Values for One Key
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "delete", tags: new[] { "name" }, Summary = "Deletes the values for one key", Description = "Deletes the values for one key", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "The userId being queried", Description = "The userId being queried", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(ResultMessage), Summary = "Structured response", Description = "This returns a structured response")]
	#endregion
	[FunctionName("DeleteValues")]
	public async Task<IActionResult> DeleteValues([HttpTrigger(AuthorizationLevel.Function, "get", Route = "delete/{id}")] HttpRequest req, string id, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName} with id: {id}", LogDataSource);

			(var success, var message) = await redisHelper.DeleteOneKey(id);

			var obj = MyLogger.LogResultMessage(new ResultMessage(executionContext.FunctionName, id, message, timer), LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}

	/// <summary>
	/// Completely Wipe the Server Data
	/// </summary>
	#region /// OpenApi Specification
	[OpenApiOperation(operationId: "reset", tags: new[] { "name" }, Summary = "Resets the entire database", Description = "Resets the entire database", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(ResultMessage), Summary = "Structured response", Description = "This returns a structured response")]
	#endregion
	[FunctionName("ResetDatabase")]
	public async Task<IActionResult> ResetDatabase([HttpTrigger(AuthorizationLevel.Function, "get", Route = "resetdatabase")] HttpRequest req, ILogger log, ExecutionContext executionContext)
	{
		Stopwatch timer = new();
		timer.Start();
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName}", LogDataSource);

			(var success, var recordsDeleted, var resultMessage) = await redisHelper.DeleteAllKeys();

			var obj = MyLogger.LogResultMessage(new ResultMessage(executionContext.FunctionName, recordsDeleted, resultMessage, timer), LogDataSource);
			return new OkObjectResult(obj);
		}
		catch (Exception ex)
		{
			var error = MyLogger.LogExceptionMessage(executionContext.FunctionName, ex);
			return new BadRequestObjectResult(error);
		}
	}
}
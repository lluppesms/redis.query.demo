// ------------------------------------------------------------------------------------------------------------------------
// Interaction_Trigger: Handle the http requests and queue up the process
// ------------------------------------------------------------------------------------------------------------------------
namespace Graph.Query;

public class Http_Trigger_Hello
{
	#region Variables
	private readonly string LogDataSource = Constants.DataSource.Triggers.Hello;
	#endregion

	#region Initialization
	/// <summary>
	/// Initializer
	/// </summary>
	public Http_Trigger_Hello(ILogger<Http_Trigger_Hello> logger)
	{
		MyLogger.InitializeLogger(logger);
	}
	#endregion

	/// <summary>
	/// Sample Hello World Method
	/// </summary>
	[OpenApiOperation(operationId: "hello", tags: new[] { "name" }, Summary = "Hello World Test", Description = "Hello World Test", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "A name", Description = "A name", Visibility = OpenApiVisibilityType.Important)]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
	[FunctionName("Hello")]
	public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "hello")] HttpRequest req, ILogger log, ExecutionContext executionContext)
	{
		try
		{
			MyLogger.Initialize_And_Log(log, $"{LogDataSource} started {executionContext.FunctionName}.", LogDataSource);
			var name = await Common.ParseStringFromRequest(req, "name");
			var responseName = string.IsNullOrEmpty(name) ? $"who are you? Please supply a name in the URL or Body" : name;
			var responseMessage = $"Hello, {responseName}. ({executionContext.FunctionName} executed successfully!)";
			MyLogger.LogInfo($"{responseMessage}", LogDataSource);
			return new OkObjectResult(responseMessage);
		}
		catch (Exception ex)
		{
			var errorMsg = MyLogger.LogException($"Error in {executionContext.FunctionName}:", ex);
			return new BadRequestObjectResult(errorMsg);
		}
	}
}

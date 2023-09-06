using System.Diagnostics;

namespace Graph.Query.Models;

public class ResultMessage
{
	[JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
	public string MethodName { get; set; }

	[JsonProperty("userName", NullValueHandling = NullValueHandling.Ignore)]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
	public string? UserName { get; set; }

	[JsonProperty("recordsRead", NullValueHandling = NullValueHandling.Ignore)]
	public int? RecordsRead { get; set; }

	[JsonProperty("recordsReturned", NullValueHandling = NullValueHandling.Ignore)]
	public int? RecordsReturned { get; set; }

	[JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
	public string Message { get; set; }

	[JsonProperty("success", NullValueHandling = NullValueHandling.Ignore)]
	public bool? Success { get; set; }

	[JsonProperty("elapsedTime", NullValueHandling = NullValueHandling.Ignore)]
	public string? ElapsedTime { get; set; }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

	public ResultMessage()
	{

	}
	public ResultMessage(string message, Stopwatch timer = null)
	{
		Message = message;
		SetElapsedTime(timer);
	}
	public ResultMessage(string methodName, string message, Stopwatch timer = null)
	{
		MethodName = methodName;
		Message = message;
		SetElapsedTime(timer);
	}
	public ResultMessage(bool success, string message, Stopwatch timer = null)
	{
		Success = success;
		Message = message;
		SetElapsedTime(timer);
	}
	public ResultMessage(string methodName, string userName, string message, Stopwatch timer = null)
	{
		MethodName = methodName;
		UserName = userName;
		Message = message;
		SetElapsedTime(timer);
	}
	public ResultMessage(string methodName, int recordsReturned, string message, Stopwatch timer = null)
	{
		MethodName = methodName;
		RecordsReturned = recordsReturned;
		Message = message;
		SetElapsedTime(timer);
	}
	public ResultMessage(string methodName, string userName, int recordsReturned, string message, Stopwatch timer = null)
	{
		MethodName = methodName;
		UserName = userName;
		RecordsReturned = recordsReturned;
		Message = message;
		SetElapsedTime(timer);
	}
	public ResultMessage(string methodName, string userName, int recordsRead, int recordsReturned, string message, Stopwatch timer = null)
	{
		MethodName = methodName;
		UserName = userName;
		RecordsRead = recordsRead;
		RecordsReturned = recordsReturned;
		Message = message;
		SetElapsedTime(timer);
	}
	public ResultMessage(string methodName, string userName, int recordsRead, int recordsReturned, string message, bool success, Stopwatch timer = null)
	{
		MethodName = methodName;
		UserName = userName;
		RecordsRead = recordsRead;
		RecordsReturned = recordsReturned;
		Message = message;
		Success = success;
		SetElapsedTime(timer);
	}

	private void SetElapsedTime(Stopwatch timer)
	{
		if (timer != null)
		{
			timer.Stop();
			var seconds = Convert.ToDecimal(timer.ElapsedMilliseconds) / 1000m;
			ElapsedTime = $"{seconds:0.0} seconds";
		}
	}
}

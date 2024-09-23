using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SailthruSDK;

/// <summary>
/// Represents a Sailthru response with payload data.
/// </summary>
/// <param name="method">The HTTP method requested.</param>
/// <param name="uri">The URI requested.</param>
/// <param name="isSuccess">States whether the status code is a success HTTP status code.</param>
/// <param name="statusCode">The HTTP status code.</param>
/// <param name="error">The API error, if available.</param>
/// <typeparam name="TData">The data type.</typeparam>
[DebuggerDisplay("{ToDebuggerString(),nq}")]
public class SailthruResponse(
HttpMethod method,
Uri uri,
bool isSuccess,
HttpStatusCode statusCode,
Error? error = default)
{
	/// <summary>
	/// Gets whether the status code represents a success HTTP status code.
	/// </summary>
	public bool IsSuccess => isSuccess;

	/// <summary>
	/// Gets the error.
	/// </summary>
	public Error? Error => error;

	/// <summary>
	/// Gets the HTTP status code of the response.
	/// </summary>
	public HttpStatusCode StatusCode => statusCode;

	/// <summary>
	/// Gets or sets the request HTTP method.
	/// </summary>
	public HttpMethod RequestMethod => method;

	/// <summary>
	/// Gets or sets the request URI.
	/// </summary>
	public Uri RequestUri => uri;

	/// <summary>
	/// Gets or sets the request content, when logging is enabled.
	/// </summary>
	public string? RequestContent { get; set; }

	/// <summary>
	/// Gets or sets the response content, when logging is enabled.
	/// </summary>
	public string? ResponseContent { get; set; }

	/// <summary>
	/// Provides a string representation for debugging.
	/// </summary>
	/// <returns></returns>
	public virtual string ToDebuggerString()
	{
			var builder = new StringBuilder();
			builder.Append($"{StatusCode}: {RequestMethod} {RequestUri.PathAndQuery}");
			if (Error is not null)
			{
					builder.Append($" - {Error.Message}");
			}

			return builder.ToString();
	}
}

/// <summary>
/// Represents a Sailthru response with payload data.
/// </summary>
/// <param name="method">The HTTP method requested.</param>
/// <param name="uri">The URI requested.</param>
/// <param name="isSuccess">States whether the status code is a success HTTP status code.</param>
/// <param name="statusCode">The HTTP status code.</param>
/// <param name="data">The API response data, if available.</param>
/// <param name="error">The API error, if available.</param>
/// <typeparam name="TData">The data type.</typeparam>
public class SailthruResponse<TData>(
HttpMethod method,
Uri uri,
bool isSuccess,
HttpStatusCode statusCode,
TData? data = default,
Error? error = default) : SailthruResponse(method, uri, isSuccess, statusCode, error)
{
	/// <summary>
	/// Gets the response data.
	/// </summary>
	public TData? Data => data;

	/// <summary>
	/// Gets whether the response has data.
	/// </summary>
	public bool HasData => data is not null;
}

/// <summary>
/// Represents a Sailthru error response.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="errors">The set of additional error messages, these may be field specific.</param>
/// <param name="exception">The exception that was caught.</param>
public class Error(string message, Dictionary<string, string[]>? errors = null, Exception? exception = null)
{
	/// <summary>
	/// Gets the set of additional error messages, these may be field specific.
	/// </summary>
	public Dictionary<string, string[]>? Errors => errors;

	/// <summary>
	/// Gets the exception that was caught.
	/// </summary>
	public Exception? Exception => exception;

	/// <summary>
	/// Gets the error message.
	/// </summary>
	public string Message => message;
}

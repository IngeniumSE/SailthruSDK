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
	/// The start of Sailthru's error message when a purchase with the same extid already exists:
	/// "Duplicate extid: [extid] ID already exists".
	/// </summary>
	public const string DuplicateExtIdMessage = "Duplicate extid";

	/// <summary>
	/// Initialises a new instance of <see cref="Error"/> with Sailthru's numeric error code.
	/// </summary>
	/// <param name="code">The Sailthru error code (the <c>error</c> value of the response), if available.</param>
	/// <param name="message">The error message (the <c>errormsg</c> value of the response).</param>
	/// <param name="errors">The set of additional error messages, these may be field specific.</param>
	/// <param name="exception">The exception that was caught.</param>
	public Error(int? code, string message, Dictionary<string, string[]>? errors = null, Exception? exception = null)
		: this(message, errors, exception)
	{
		Code = code;
	}

	/// <summary>
	/// Gets the Sailthru error code (the <c>error</c> value of the response), if available.
	/// </summary>
	public int? Code { get; }

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

	/// <summary>
	/// Gets whether Sailthru rejected a purchase because a purchase with the same extid already exists.
	/// For a retried purchase this means Sailthru already has it.
	/// </summary>
	public bool IsDuplicateExtId
		=> Message is { Length: > 0 } && Message.IndexOf(DuplicateExtIdMessage, StringComparison.OrdinalIgnoreCase) >= 0;

	/// <inheritdoc />
	public override string ToString()
		=> Code.HasValue ? $"Sailthru error {Code.Value}: {Message}" : Message;
}

/// <summary>
/// Represents an unsuccessful Sailthru API response, thrown by <see cref="SailthruResponseExtensions.EnsureSuccess{TResponse}(TResponse)"/>.
/// </summary>
public class SailthruException : Exception
{
	/// <summary>
	/// Initialises a new instance of <see cref="SailthruException"/>.
	/// </summary>
	/// <param name="response">The unsuccessful response.</param>
	public SailthruException(SailthruResponse response)
		: base(
			Ensure.IsNotNull(response, nameof(response)).Error?.ToString() ?? $"The Sailthru API returned HTTP {(int)response.StatusCode}.",
			response.Error?.Exception)
	{
		Response = response;
	}

	/// <summary>
	/// Gets the response.
	/// </summary>
	public SailthruResponse Response { get; }

	/// <summary>
	/// Gets the error, if available.
	/// </summary>
	public Error? Error => Response.Error;

	/// <summary>
	/// Gets the Sailthru error code, if available.
	/// </summary>
	public int? Code => Response.Error?.Code;

	/// <summary>
	/// Gets the HTTP status code (0 if no response was received).
	/// </summary>
	public HttpStatusCode StatusCode => Response.StatusCode;
}

/// <summary>
/// Provides extensions for the <see cref="SailthruResponse"/> type.
/// </summary>
public static class SailthruResponseExtensions
{
	/// <summary>
	/// Throws a <see cref="SailthruException"/> if the response is not successful.
	/// </summary>
	/// <typeparam name="TResponse">The response type.</typeparam>
	/// <param name="response">The response.</param>
	/// <returns>The response, if successful.</returns>
	/// <exception cref="SailthruException">The response is not successful.</exception>
	public static TResponse EnsureSuccess<TResponse>(this TResponse response)
		where TResponse : SailthruResponse
	{
		Ensure.IsNotNull(response, nameof(response));

		if (!response.IsSuccess)
		{
			throw new SailthruException(response);
		}

		return response;
	}
}

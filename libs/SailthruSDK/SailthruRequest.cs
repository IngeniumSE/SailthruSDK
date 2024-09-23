namespace SailthruSDK;

using System.Net.Http;

/// <summary>
/// Represents a request to a Sailthru API resource.
/// </summary>
/// <param name="method">The HTTP method.</param>
/// <param name="resource">The relative resource.</param>
/// <param name="query">The query string.</param>
public class SailthruRequest(
	HttpMethod method,
	PathString resource,
	QueryString? query = null)
{
	/// <summary>
	/// Gets the HTTP method for the request.
	/// </summary>
	public HttpMethod Method => method;

	/// <summary>
	/// Gets the relative resource for the request.
	/// </summary>
	public PathString Resource => resource;

	/// <summary>
	/// Gets the query string.
	/// </summary>
	public QueryString? Query => query;
}

/// <summary>
/// Represents a request to a Sailthru API resource.
/// </summary>
/// <param name="method">The HTTP method.</param>
/// <param name="resource">The relative resource.</param>
/// <param name="data">The data.</param>
/// <typeparam name="TData">The data type.</typeparam>
public class SailthruRequest<TData>(
	HttpMethod method,
	PathString resource,
	TData data,
	QueryString? query = null) : SailthruRequest(method, resource, query)
	where TData : notnull
{
	/// <summary>
	/// Gets the model for the request.
	/// </summary>
	public TData Data => data;
}


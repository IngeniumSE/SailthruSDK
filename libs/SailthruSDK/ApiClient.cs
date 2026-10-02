// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SailthruSDK;

/// <summary>
/// Provides a base implementation of an API client.
/// </summary>
public abstract class ApiClient
{
	readonly HttpClient _http;
	readonly SailthruSettings _settings;
	readonly JsonSerializerOptions _serializerOptions = JsonUtility.CreateSerializerOptions();
	readonly Uri _baseUrl;

	protected ApiClient(HttpClient http, SailthruSettings settings, string baseUrl)
	{
		_http = Ensure.IsNotNull(http, nameof(http));
		_settings = Ensure.IsNotNull(settings, nameof(settings));
		_baseUrl = new Uri(baseUrl);
	}

	#region Send and Fetch
	protected internal async Task<SailthruResponse> SendAsync(
		SailthruRequest request,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNull(request, nameof(request));
		var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformResponse(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp)
				.ConfigureAwait(false);

			if (_settings.CaptureRequestContent && httpReq.Content is not null)
			{
				transformedResponse.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (_settings.CaptureResponseContent && httpResp.Content is not null)
			{
				transformedResponse.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return transformedResponse;
		}
		catch (Exception ex)
		{
			var response = new SailthruResponse(
				httpReq.Method,
				httpReq.RequestUri,
				false,
				(HttpStatusCode)0,
				error: new Error(ex.Message, exception: ex));

			if (httpReq?.Content is not null)
			{
				response.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (httpResp?.Content is not null)
			{
				response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return response;
		}
	}

	protected internal async Task<SailthruResponse> SendAsync<TRequest>(
		SailthruRequest<TRequest> request,
		CancellationToken cancellationToken = default)
		where TRequest : notnull
	{
		Ensure.IsNotNull(request, nameof(request));
		var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken);

			var transformedResponse = await TransformResponse(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp)
					.ConfigureAwait(false); ;

			if (_settings.CaptureRequestContent && httpReq.Content is not null)
			{
				transformedResponse.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (_settings.CaptureResponseContent && httpResp.Content is not null)
			{
				transformedResponse.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			return transformedResponse;
		}
		catch (Exception ex)
		{
			var response = new SailthruResponse(
				httpReq.Method,
				httpReq.RequestUri,
				false,
				(HttpStatusCode)0,
				error: new Error(ex.Message, exception: ex));

			if (httpReq?.Content is not null)
			{
				response.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (httpResp?.Content is not null)
			{
				response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return response;
		}
	}

	protected internal async Task<SailthruResponse<TResponse>> FetchAsync<TResponse>(
		SailthruRequest request,
		CancellationToken cancellationToken = default)
		where TResponse : class
	{
		Ensure.IsNotNull(request, nameof(request));
		var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp)
					.ConfigureAwait(false); ;

			if (_settings.CaptureRequestContent && httpReq.Content is not null)
			{
				transformedResponse.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			if (_settings.CaptureResponseContent && httpResp.Content is not null)
			{
				transformedResponse.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			return transformedResponse;
		}
		catch (Exception ex)
		{
			var response = new SailthruResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				false,
				(HttpStatusCode)0,
				error: new Error(ex.Message, exception: ex));

			if (httpReq?.Content is not null)
			{
				response.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (httpResp?.Content is not null)
			{
				response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return response;
		}
	}

	protected internal async Task<SailthruResponse<TResponse>> FetchAsync<TRequest, TResponse>(
		SailthruRequest<TRequest> request,
		CancellationToken cancellationToken = default)
		where TRequest : notnull
		where TResponse : class
	{
		Ensure.IsNotNull(request, nameof(request));
		var httpReq = CreateHttpRequest(request);
		HttpResponseMessage? httpResp = null;

		try
		{
			httpResp = await _http.SendAsync(httpReq, cancellationToken)
				.ConfigureAwait(false);

			var transformedResponse = await TransformResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				httpResp)
					.ConfigureAwait(false); ;

			if (_settings.CaptureRequestContent && httpReq.Content is not null)
			{
				transformedResponse.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (_settings.CaptureResponseContent && httpResp.Content is not null)
			{
				transformedResponse.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			return transformedResponse;
		}
		catch (Exception ex)
		{
			var response = new SailthruResponse<TResponse>(
				httpReq.Method,
				httpReq.RequestUri,
				false,
				(HttpStatusCode)0,
				error: new Error(ex.Message, exception: ex));

			if (httpReq?.Content is not null)
			{
				response.RequestContent = await httpReq.Content.ReadAsStringAsync()
					.ConfigureAwait(false);
			}

			if (httpResp?.Content is not null)
			{
				response.ResponseContent = await httpResp.Content.ReadAsStringAsync()
					.ConfigureAwait(false); ;
			}

			return response;
		}
	}
	#endregion

	#region Preprocessing
	protected internal HttpRequestMessage CreateHttpRequest(
		SailthruRequest request)
	{
		string pathAndQuery = request.Resource.ToUriComponent();
		var query = CreateQueryString(request.Method);
		if (query != null)
		{
			pathAndQuery += query.Value.ToUriComponent();
		}
		var uri = new Uri(_baseUrl, pathAndQuery);

		var message = new HttpRequestMessage(request.Method, uri);

		return message;
	}

	protected internal HttpRequestMessage CreateHttpRequest<TRequest>(
		SailthruRequest<TRequest> request)
		where TRequest : notnull
	{
		string pathAndQuery = request.Resource.ToUriComponent();
		var query = CreateQueryString(request.Method, request.Data);
		if (query != null)
		{
			pathAndQuery += query.Value.ToUriComponent();
		}
		var uri = new Uri(_baseUrl, pathAndQuery);

		var message = new HttpRequestMessage(request.Method, uri);

		message.Content = CreateHttpContent(request.Method, request.Data);

		return message;
	}
	#endregion

	#region Postprocessing
	protected internal async Task<SailthruResponse> TransformResponse(
		HttpMethod method,
		Uri uri,
		HttpResponseMessage response,
		CancellationToken cancellationToken = default)
	{
		if (response.IsSuccessStatusCode)
		{
			return new SailthruResponse(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode);
		}
		else
		{
			Error? error = await ReadErrorAsync(response)
				.ConfigureAwait(false);

			return new SailthruResponse(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				error: error
			);
		}
	}

	protected internal async Task<SailthruResponse<TResponse>> TransformResponse<TResponse>(
		HttpMethod method,
		Uri uri,
		HttpResponseMessage response,
		CancellationToken cancellationToken = default)
		where TResponse : class
	{
		if (response.IsSuccessStatusCode)
		{
			TResponse? data = default;
			if (response.Content is not null)
			{
				data = await response.Content.ReadFromJsonAsync<TResponse>(
					_serializerOptions, cancellationToken)
					.ConfigureAwait(false);
			}

			return new SailthruResponse<TResponse>(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				data: data
			);
		}
		else
		{
			Error? error = await ReadErrorAsync(response)
				.ConfigureAwait(false);

			return new SailthruResponse<TResponse>(
				method,
				uri,
				response.IsSuccessStatusCode,
				response.StatusCode,
				error: error
			);
		}
	}

	string? GetHeader(string name, HttpHeaders headers)
		=> headers.TryGetValues(name, out var values)
		? values.First()
		: null;

	/// <summary>
	/// Reads the error from an unsuccessful response. Never throws for a malformed or non-JSON body.
	/// </summary>
	internal static async Task<Error> ReadErrorAsync(HttpResponseMessage response)
	{
		if (response.Content is null)
		{
			return new Error(Resources.ApiClient_NoErrorMessage);
		}

		string body = await response.Content.ReadAsStringAsync()
			.ConfigureAwait(false);

		return ParseError(body);
	}

	/// <summary>
	/// Parses a Sailthru error body: <c>{ "error": 14, "errormsg": "..." }</c>.
	/// A <c>message</c> / <c>errors</c> body is also accepted.
	/// </summary>
	/// <param name="body">The response body.</param>
	/// <returns>The error.</returns>
	public static Error ParseError(string? body)
	{
		if (body is not { Length: > 0 } || string.IsNullOrWhiteSpace(body))
		{
			return new Error(Resources.ApiClient_NoErrorMessage);
		}

		try
		{
			using var document = JsonDocument.Parse(body);
			var root = document.RootElement;

			if (root.ValueKind == JsonValueKind.Object)
			{
				int? code = root.TryGetProperty("error", out var codeElement)
					&& codeElement.ValueKind == JsonValueKind.Number
					&& codeElement.TryGetInt32(out var number)
					? number
					: null;

				string? message = GetString(root, "errormsg") ?? GetString(root, "message");

				return new Error(
					code,
					message ?? Resources.ApiClient_UnknownResponse,
					GetErrors(root));
			}
		}
		catch (JsonException)
		{
			// Not JSON (e.g. a proxy error page).
		}

		return new Error(Resources.ApiClient_UnknownResponse);

		static string? GetString(JsonElement root, string name)
			=> root.TryGetProperty(name, out var element)
				&& element.ValueKind == JsonValueKind.String
				&& element.GetString() is { Length: > 0 } value
				? value
				: null;

		static Dictionary<string, string[]>? GetErrors(JsonElement root)
		{
			if (!root.TryGetProperty("errors", out var element) || element.ValueKind != JsonValueKind.Object)
			{
				return null;
			}

			var errors = new Dictionary<string, string[]>();
			foreach (var property in element.EnumerateObject())
			{
				errors[property.Name] = property.Value.ValueKind switch
				{
					JsonValueKind.Array => property.Value.EnumerateArray()
						.Where(v => v.ValueKind == JsonValueKind.String)
						.Select(v => v.GetString()!)
						.ToArray(),
					JsonValueKind.String => [property.Value.GetString()!],
					_ => [property.Value.GetRawText()]
				};
			}

			return errors;
		}
	}
	#endregion

	protected internal Lazy<TOperations> Defer<TOperations>(Func<ApiClient, TOperations> factory)
		=> new Lazy<TOperations>(() => factory(this));

	protected internal Uri Root(string resource)
		=> new Uri(resource, UriKind.Relative);

	QueryString? CreateQueryString<TData>(HttpMethod method, TData? data = default)
	{
		if (method != HttpMethod.Post)
		{
			string json = JsonSerializer.Serialize(data, _serializerOptions);
			var signature = SignatureGenerator.Generate(_settings.ApiKey, _settings.ApiSecret, payload: json);

			var builder = new QueryStringBuilder();
			builder.AddParameter("api_key", _settings.ApiKey);
			builder.AddParameter("sig", signature);
			builder.AddParameter("format", "json");
			builder.AddParameter("json", json);

			return builder.Build();
		}

		return null;
	}

	QueryString? CreateQueryString(HttpMethod method)
	{
		if (method != HttpMethod.Post)
		{
			var signature = SignatureGenerator.Generate(_settings.ApiKey, _settings.ApiSecret);

			var builder = new QueryStringBuilder();
			builder.AddParameter("api_key", _settings.ApiKey);
			builder.AddParameter("sig", signature);
			builder.AddParameter("format", "json");

			return builder.Build();
		}

		return null;
	}

	HttpContent? CreateHttpContent<TData>(HttpMethod method, TData? data = default)
	{
		if (method == HttpMethod.Post)
		{
			string json = JsonSerializer.Serialize(data, _serializerOptions);
			var signature = SignatureGenerator.Generate(_settings.ApiKey, _settings.ApiSecret, payload: json);

			var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
			{
					new("api_key", _settings.ApiKey),
					new("sig", signature),
					new("format", "json"),
					new("json", json)
			});

			return content;
		}

		return null;
	}
}

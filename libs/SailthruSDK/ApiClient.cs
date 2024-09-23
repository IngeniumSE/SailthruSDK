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
		async Task<Error> GetSailthruError()
		{
			Error error;
			if (response.Content is not null)
			{
				var result = await response.Content.ReadFromJsonAsync<ErrorContainer>(cancellationToken)
					.ConfigureAwait(false);

				if (result?.Message is not { Length: > 0 })
				{
					error = new(Resources.ApiClient_UnknownResponse, result?.Errors);
				}
				else
				{
					error = new(result.Message, result.Errors);
				}
			}
			else
			{
				error = new Error(Resources.ApiClient_NoErrorMessage);
			}

			return error;
		}

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
			Error? error = await GetSailthruError();

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
		async Task<Error> GetSailthruError()
		{
			Error error;
			if (response.Content is not null)
			{
				var result = await response.Content.ReadFromJsonAsync<ErrorContainer>(
					(JsonSerializerOptions?)null, cancellationToken)
					.ConfigureAwait(false);

				if (result?.Message is not { Length: > 0 })
				{
					error = new(Resources.ApiClient_UnknownResponse, result?.Errors);
				}
				else
				{
					error = new(result.Message, result.Errors);
				}
			}
			else
			{
				error = new Error(Resources.ApiClient_NoErrorMessage);
			}

			return error;
		}

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
			Error? error = await GetSailthruError();

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

	class ErrorContainer
	{
		[JsonPropertyName("errors")]
		public Dictionary<string, string[]>? Errors { get; set; }

		[JsonPropertyName("message")]
		public string Message { get; set; } = default!;
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

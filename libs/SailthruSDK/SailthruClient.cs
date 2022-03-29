namespace SailthruSDK
{
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Text.Json;
	using System.Text.Json.Serialization;
	using System.Threading;
	using System.Threading.Tasks;

	using static System.Net.WebUtility;

	/// <summary>
	/// Provides a client for integrating the Sailthru API.
	/// </summary>
	public class SailthruClient
	{
		readonly HttpClient _http;
		readonly SailthruSettings _settings;
		static readonly JsonSerializerOptions _jsonOptions = JsonUtility.GetSerializerOptions();

		/// <summary>
		/// Initialises a new instance of <see cref="SailthruClient"/>
		/// </summary>
		/// <param name="http">The HTTP client.</param>
		/// <param name="settings">The Sailthru settings.</param>
		public SailthruClient(HttpClient http, SailthruSettings settings)
		{
			_http = Ensure.IsNotNull(http, nameof(http));
			_settings = Ensure.IsNotNull(settings, nameof(settings));
		}

		internal async Task<SailthruResponse<TResponse>> SendAsync<TRequest, TResponse>(
			SailthruRequest<TRequest> request,
			CancellationToken cancellationToken = default)
			where TRequest : notnull
		{
			var httpResponse = await GetHttpResponseAsync(request, cancellationToken);

			if (httpResponse.IsSuccessStatusCode)
			{
				string content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

				return SailthruResponse<TResponse>.Success(
					await JsonSerializer.DeserializeAsync<TResponse>(
						await httpResponse.Content.ReadAsStreamAsync(),
						_jsonOptions,
						cancellationToken: cancellationToken
					)
				);
			}

			var error = await JsonSerializer.DeserializeAsync<Error>(
				await httpResponse.Content.ReadAsStreamAsync(),
				_jsonOptions,
				cancellationToken: cancellationToken)
				?? new Error { Code = -1, Message = "Unknown error response." };

			return SailthruResponse<TResponse>.Failure(new SailthruError(error.Code, error.Message));
		}

		internal async Task<SailthruResponse> SendAsync<TRequest>(
			SailthruRequest<TRequest> request,
			CancellationToken cancellationToken = default)
			where TRequest : notnull
		{
			var httpResponse = await GetHttpResponseAsync(request, cancellationToken);

			if (httpResponse.IsSuccessStatusCode)
			{
				string content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

				return SailthruResponse.Success();
			}

			var error = await JsonSerializer.DeserializeAsync<Error>(
				await httpResponse.Content.ReadAsStreamAsync(),
				_jsonOptions,
				cancellationToken: cancellationToken)
				?? new Error { Code = -1, Message = "Unknown error response." };

			return SailthruResponse.Failure(new SailthruError(error.Code, error.Message));
		}

		async Task<HttpResponseMessage> GetHttpResponseAsync<TRequest>(
			SailthruRequest<TRequest> request,
			CancellationToken cancellationToken)
			where TRequest : notnull
		{
			Ensure.IsNotNull(request, nameof(request));

			string json = JsonSerializer.Serialize(request.Model, _jsonOptions);
			string signature = SignatureGenerator.Generate(_settings.ApiKey, _settings.ApiSecret, payload: json);
			var requestUri = $"/{request.Endpoint}";
			if (request.Method != HttpMethod.Post)
			{
				requestUri += $"?api_key={UrlEncode(_settings.ApiKey)}&sig={UrlEncode(signature)}&format=json&json={UrlEncode(json)}";
			}

			var httpRequest = new HttpRequestMessage(
				request.Method,
				new Uri(_http.BaseAddress, requestUri));

			if (request.Method == HttpMethod.Post)
			{
				httpRequest.Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
				{
					new("api_key", _settings.ApiKey),
					new("sig", signature),
					new("format", "json"),
					new("json", json)
				});
			}

			var httpResponse = await _http.SendAsync(httpRequest, cancellationToken)
				.ConfigureAwait(false);

			return httpResponse;
		}

		public class Error
		{
			[JsonPropertyName("error")]
			public int Code { get; set; }
			[JsonPropertyName("errormsg")]
			public string Message { get; set; } = default!;
		}
	}
}
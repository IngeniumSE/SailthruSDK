namespace SailthruSDK
{
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Text.Json;
	using System.Threading;
	using System.Threading.Tasks;

  using Microsoft.Extensions.Options;

	using static System.Net.WebUtility;

  /// <summary>
  /// Provides a client for integrating the Sailthru API.
  /// </summary>
  public class SailthruClient
	{
		readonly HttpClient _http;
		readonly SailthruSettings _settings;
		readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
		{
			WriteIndented = false,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		/// <summary>
		/// Initialises a new instance of <see cref="SailthruClient"/>
		/// </summary>
		/// <param name="http">The HTTP client.</param>
		/// <param name="settings">The Sailthru settings.</param>
		public SailthruClient(HttpClient http, IOptions<SailthruSettings> settings)
		{
			_http = Ensure.IsNotNull(http, nameof(http));
			_settings = Ensure.IsNotNull(settings, nameof(settings)).Value;
		}

		internal async Task<SailthruResponse> SendAsync<TModel>(SailthruRequest<TModel> request, CancellationToken cancellationToken = default)
			where TModel : notnull
		{
			Ensure.IsNotNull(request, nameof(request));

			string json = JsonSerializer.Serialize(request.Model, _jsonOptions);
			string signature = SignatureGenerator.Generate(_settings.ApiKey, _settings.ApiSecret, json);
			var requestUri = $"/{request.Endpoint}?api_key={UrlEncode(_settings.ApiKey)}&sig={UrlEncode(signature)}&format=json&json={UrlEncode(json)}";

			var httpRequest = new HttpRequestMessage(
				request.Method,
				new Uri(_http.BaseAddress, requestUri));

			var httpResponse = await _http.SendAsync(httpRequest, cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
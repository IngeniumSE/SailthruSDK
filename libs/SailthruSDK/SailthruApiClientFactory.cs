// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using SailthruSDK.Api;

namespace SailthruSDK;

public interface ISailthruApiClientFactory
{
	ISailthruApiClient CreateApiClient(
		SailthruSettings settings,
		string name = SailthruApiConstants.DefaultSailthruApiClient);
}

/// <summary>
/// Provides factory services for creating Sailthru client instances.
/// </summary>
public class SailthruApiClientFactory : ISailthruApiClientFactory
{
	readonly ISailthruHttpClientFactory _httpClientFactory;

	public SailthruApiClientFactory(ISailthruHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = Ensure.IsNotNull(httpClientFactory, nameof(httpClientFactory));
	}

	public ISailthruApiClient CreateApiClient(
		SailthruSettings settings,
		string name = SailthruApiConstants.DefaultSailthruApiClient)
		=> new SailthruApiClient(_httpClientFactory.CreateHttpClient(name), settings);
}

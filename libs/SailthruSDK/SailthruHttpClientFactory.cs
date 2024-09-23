// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace SailthruSDK;

/// <summary>
/// Provides factory methods for creating a HTTP client.
/// </summary>
public interface ISailthruHttpClientFactory
{
	/// <summary>
	/// Creates a HTTP client.
	/// </summary>
	/// <param name="name">The HTTP client name.</param>
	/// <returns>The HTTP client.</returns>
	HttpClient CreateHttpClient(string name);
}

public class SailthruHttpClientFactory(IHttpClientFactory clientFactory) : ISailthruHttpClientFactory
{
	public HttpClient CreateHttpClient(string name)
		=> Ensure.IsNotNull(clientFactory, nameof(clientFactory)).CreateClient(name);
}

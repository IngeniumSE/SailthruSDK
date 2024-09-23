// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace SailthruSDK.Api;

public partial interface ISailthruApiClient
{

}

public partial class SailthruApiClient : ApiClient, ISailthruApiClient
{
	public SailthruApiClient(HttpClient http, SailthruSettings settings)
		: base(http, settings, settings.BaseUrl) { }
}

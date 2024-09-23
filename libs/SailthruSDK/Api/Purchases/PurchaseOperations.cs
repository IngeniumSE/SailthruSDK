// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace SailthruSDK.Api;

partial interface ISailthruApiClient
{
	/// <summary>
	/// Gets the purchase operations.
	/// </summary>
	IPurchaseOperations Purchases { get; }
}

public partial class  SailthruApiClient
{
	Lazy<IPurchaseOperations>? _purchases;
	public IPurchaseOperations Purchases => (_purchases ??= Defer<IPurchaseOperations>(
		c => new PurchaseOperations(new("/purchase"), c))).Value;
}

public partial interface IPurchaseOperations
{
	Task<SailthruResponse> UpsertPurchaseAsync(
		string email,
		PurchaseItem[] items,
		bool incomplete = false,
		string? messageId = default,
		CancellationToken cancellationToken = default);
}

internal class PurchaseOperations(
	PathString path,
	ApiClient client) : IPurchaseOperations
{
	readonly PathString _path = path;
	readonly ApiClient _client = client;

	public async Task<SailthruResponse> UpsertPurchaseAsync(
		string email,
		PurchaseItem[] items,
		bool incomplete = false,
		string? messageId = default,
		CancellationToken cancellationToken = default)
	{
		var model = new UpsertPurchaseRequest(email, items, incomplete, messageId);
		var request = new SailthruRequest<UpsertPurchaseRequest>(HttpMethod.Post, _path, model);

		return await _client.SendAsync(
			request,
			cancellationToken)
			.ConfigureAwait(false);
	}
}

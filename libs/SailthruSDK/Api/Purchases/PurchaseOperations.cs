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
	/// <summary>
	/// Creates a purchase, or (when <paramref name="incomplete"/> is true) sets the user's current cart.
	/// </summary>
	/// <param name="email">The user email address.</param>
	/// <param name="items">The set of items.</param>
	/// <param name="incomplete">True for an incomplete purchase (an active cart, not an order).</param>
	/// <param name="messageId">The message ID of the email the user came from, usually the sailthru_bid cookie.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The response. Check <see cref="SailthruResponse.IsSuccess"/> and <see cref="SailthruResponse.Error"/>.</returns>
	Task<SailthruResponse> UpsertPurchaseAsync(
		string email,
		PurchaseItem[] items,
		bool incomplete = false,
		string? messageId = default,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Creates a purchase, or sets the user's current cart, with the full set of options
	/// (e.g. <see cref="UpsertPurchaseRequest.ExtId"/> and <see cref="UpsertPurchaseRequest.Date"/>).
	/// </summary>
	/// <param name="request">The purchase request.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>
	/// The response. Check <see cref="SailthruResponse.IsSuccess"/> and <see cref="SailthruResponse.Error"/>;
	/// <see cref="Error.IsDuplicateExtId"/> means Sailthru already has a purchase with this extid.
	/// </returns>
	/// <exception cref="ArgumentException">The request is not valid, see <see cref="UpsertPurchaseRequest.Validate"/>.</exception>
	Task<SailthruResponse> UpsertPurchaseAsync(
		UpsertPurchaseRequest request,
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
		=> await UpsertPurchaseAsync(
			new UpsertPurchaseRequest(email, items, incomplete, messageId),
			cancellationToken)
			.ConfigureAwait(false);

	public async Task<SailthruResponse> UpsertPurchaseAsync(
		UpsertPurchaseRequest request,
		CancellationToken cancellationToken = default)
	{
		Ensure.IsNotNull(request, nameof(request));
		request.Validate();

		var sailthruRequest = new SailthruRequest<UpsertPurchaseRequest>(HttpMethod.Post, _path, request);

		return await _client.SendAsync(
			sailthruRequest,
			cancellationToken)
			.ConfigureAwait(false);
	}
}

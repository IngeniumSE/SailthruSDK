namespace SailthruSDK
{
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;

	using SailthruSDK.Purchase;

	public static class SailthruPurchaseExtensions
	{
		public static async Task<SailthruResponse> UpsertPurchaseAsync(
			this SailthruClient client,
			string email,
			PurchaseItem[] items,
			bool incomplete = false,
			string? messageId = default,
			CancellationToken cancellationToken = default)
		{
			Ensure.IsNotNull(client, nameof(client));
			
			var model = new UpsertPurchaseRequest(email, items, incomplete, messageId);
			var request = new SailthruRequest<UpsertPurchaseRequest>(
				HttpMethod.Post,
				SailthruEndpoints.Purchase,
				model);

			var response = await client.SendAsync(request, cancellationToken)
				.ConfigureAwait(false);

			return response;
		}
	}
}
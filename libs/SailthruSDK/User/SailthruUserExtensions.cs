namespace SailthruSDK
{
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;

	using SailthruSDK.User;

	/// <summary>
	/// Provides extensions for managing Sailthru users.
	/// </summary>
	public static class SailthruUserExtensions
	{
		/// <summary>
		/// Gets the user with the given ID.
		/// </summary>
		/// <param name="client">The Sailthru client.</param>
		/// <param name="id">The user ID.</param>
		/// <param name="key">The key type.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The user instance, or null if it does not match a valid Sailthru user.</returns>
		public static async Task<SailthruUser?> GetUserAsync(
			this SailthruClient client,
			string id,
			string key = SailthruUserKeyType.Email,
			SailthruUserFields? fields = default,
			CancellationToken cancellationToken = default)
		{
			Ensure.IsNotNull(client, nameof(client));
			Ensure.IsNotNullOrEmpty(id, nameof(id));

			var model = new GetUserRequest(id, key, fields);
			var request = new SailthruRequest<GetUserRequest>(
				HttpMethod.Get,
				SailthruEndpoints.User,
				model);

			var response = await client.SendAsync<GetUserRequest, SailthruUser>(
				request,
				cancellationToken)
				.ConfigureAwait(false);

			if (response is { IsError: true })
			{
				return default;
			}

			return response.Result;
		}

		/// <summary>
		/// Creates or updates an existing user.
		/// </summary>
		/// <param name="client">The Sailthru client.</param>
		/// <param name="id">The user ID.</param>
		/// <param name="key">The key type.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The user instance, or null if it does not match a valid Sailthru user.</returns>
		public static async Task<SailthruUser?> UpsertUserAsync(
			this SailthruClient client,
			string id,
			string key = SailthruUserKeyType.Email,
			Map<string>? keys = default,
			KeyConflict keyConflict = KeyConflict.Merge,
			Map<string>? cookies = default,
			Map<bool>? lists = default,
			Map<bool>? templates = default,
			Map<string>? vars = default,
			OptOutStatus? optOutEmailStatus = default,
			bool? optOutSms = default,
			SailthruUserFields? fields = default,
			CancellationToken cancellationToken = default)
		{
			Ensure.IsNotNull(client, nameof(client));
			Ensure.IsNotNullOrEmpty(id, nameof(id));

			var model = new UpsertUserRequest(id, key, keys, keyConflict, cookies, lists, templates, vars, optOutEmailStatus, optOutSms, fields);
			var request = new SailthruRequest<UpsertUserRequest>(
				HttpMethod.Post,
				SailthruEndpoints.User,
				model);

			var response = await client.SendAsync<UpsertUserRequest, SailthruUser>(
				request,
				cancellationToken)
				.ConfigureAwait(false);

			if (response is { IsError: true })
			{
				return default;
			}

			return response.Result;
		}
	}
}

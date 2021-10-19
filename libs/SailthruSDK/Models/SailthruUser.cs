namespace SailthruSDK
{
	using System;
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Represents a Sailthru user.
	/// </summary>
	public class SailthruUser
	{
		/// <summary>
		/// Gets recent user activity.
		/// </summary>
		public SailthruUserActivity? Activity { get; }
	}

	/// <summary>
	/// Represents sailthru user activity.
	/// </summary>
	public struct SailthruUserActivity
	{
		/// <summary>
		/// The date and time the user's most recent click.
		/// </summary>
		public DateTimeOffset? ClickTime { get; }

		/// <summary>
		/// The date and time of the user's profile creation.
		/// </summary>
		public DateTimeOffset? CreateTime { get; }

		/// <summary>
		/// The date and time the user's most recent log in.
		/// </summary>
		public DateTimeOffset? LoginTime { get; }

		/// <summary>
		/// The date and time the user's most recent email open.
		/// </summary>
		public DateTimeOffset? OpenTime { get; }

		/// <summary>
		/// The date and time the user was added to their first list.
		/// </summary>
		public DateTimeOffset? SignupTime { get; }

		/// <summary>
		/// The date and time the user's most recent view.
		/// </summary>
		public DateTimeOffset? ViewTime { get; }
	}

	/// <summary>
	/// Represenst a request to get a user.
	/// </summary>
	public class GetUserRequest
	{
		public GetUserRequest(
			string id,
			SailthruUserKeyType key = SailthruUserKeyType.Email,
			SailthruUserFields? fields = default)
		{
			Id = Ensure.IsNotNull(id, nameof(id));
			Key = key;
			Fields = ToMap(fields);
		}

		public string Id { get; }
		public SailthruUserKeyType Key { get; }
		public Map<bool>? Fields { get; }

		Map<bool>? ToMap(SailthruUserFields? fields)
		{
			if (fields is null)
			{
				return default;
			}

			var map = new Map<bool>();
			Map(map, "activity", fields.Activity);
			Map(map, "device", fields.Device);
			Map(map, "engagement", fields.Engagement);
			Map(map, "keys", fields.Keys);
			Map(map, "lifetime", fields.Lifetime);
			Map(map, "lists", fields.Lists);
			Map(map, "optout_email", fields.OptOutEmail);
			Map(map, "purchase_incomplete", fields.PurchaseIncomplete);
			Map(map, "purchases", fields.Purchases);
			Map(map, "smart_lists", fields.SmartLists);
			Map(map, "vars", fields.Vars);

			return map;
		}

		void Map(Map<bool> map, string name, bool value)
		{
			if (value)
			{
				map[name] = true;
			}
		}
	}

	/// <summary>
	/// Defines the possbile Sailthru user key types.
	/// </summary>
	public enum SailthruUserKeyType
	{
		Email,
		SailthruId,
		ExternalId,
		Sms
	}

	/// <summary>
	/// Represets the Sailthru user fields to return.
	/// </summary>
	public class SailthruUserFields
	{
		public bool Activity;
		public bool Device;
		public bool Engagement;
		public bool Keys;
		public bool Lifetime;
		public bool Lists;
		public bool OptOutEmail;
		public bool PurchaseIncomplete;
		public bool Purchases;
		public bool SmartLists;
		public bool Vars;
	}

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
			SailthruUserKeyType key = SailthruUserKeyType.Email,
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
	}
}

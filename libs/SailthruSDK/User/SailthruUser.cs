namespace SailthruSDK.User
{
	using System;
	using System.Text.Json.Serialization;

	using SailthruSDK.Converters;

	/// <summary>
	/// Represents a Sailthru user.
	/// </summary>
	public class SailthruUser
	{
		/// <summary>
		/// Gets recent user activity.
		/// </summary>
		[JsonPropertyName("activity")]
		public SailthruUserActivity? Activity { get; set; }

		/// <summary>
		/// Gets or sets the device.
		/// </summary>
		[JsonPropertyName("device")]
		public SailthruUserDevice? Device { get; set; }

		/// <summary>
		/// Gets or sets the engagement.
		/// </summary>
		[JsonPropertyName("engagement")]
		public string? Engagement { get; set; }

		/// <summary>
		/// Gets or sets the email opt-out status.
		/// </summary>
		[JsonPropertyName("optout_email")]
		public OptOutStatus? OptOutStatus { get; set; }

		/// <summary>
		/// Gets or sets the set of keys associated with the user.
		/// </summary>
		[JsonPropertyName("keys")]
		public Map<string>? Keys { get; set; }

		/// <summary>
		/// Gets or sets lifetime stats about the user.
		/// </summary>
		[JsonPropertyName("lifetime")]
		public SailthruUserLifetime? Lifetime { get; set; }

		/// <summary>
		/// Gets or sets the set of lists the user has signed up for.
		/// </summary>
		[JsonPropertyName("lists")]
		public Map<DateTimeOffset>? Lists { get; set; }

		/// <summary>
		/// Gets or sets the set of smart lists the user is included in.
		/// </summary>
		[JsonPropertyName("smart-lists")]
		public string[]? SmartLists { get; set; }

		/// <summary>
		/// Gets the set of purchases.
		/// </summary>
		[JsonPropertyName("purchases")]
		public SailthruUserPurchase[]? Purchases { get; set; }

		/// <summary>
		/// Gets the set of purchases.
		/// </summary>
		[JsonPropertyName("purchase_incomplete")]
		public SailthruUserPurchase[]? IncompletePurchases { get; set; }

		/// <summary>
		/// Gets or sets the set of variables associated with the user.
		/// </summary>
		[JsonPropertyName("vars")]
		public Map<string>? Vars { get; set; }
	}

	/// <summary>
	/// Represents sailthru user activity.
	/// </summary>
	public class SailthruUserActivity
	{
		/// <summary>
		/// The date and time the user's most recent click.
		/// </summary>
		[JsonPropertyName("click_time")]
		public DateTimeOffset? ClickTime { get; set; }

		/// <summary>
		/// The date and time of the user's profile creation.
		/// </summary>
		[JsonPropertyName("create_time")]
		public DateTimeOffset? CreateTime { get; set; }

		/// <summary>
		/// The date and time the user's most recent log in.
		/// </summary>
		[JsonPropertyName("login_time")]
		public DateTimeOffset? LoginTime { get; set; }

		/// <summary>
		/// The date and time the user's most recent email open.
		/// </summary>
		[JsonPropertyName("open_time")]
		public DateTimeOffset? OpenTime { get; set; }

		/// <summary>
		/// The date and time the user was added to their first list.
		/// </summary>
		[JsonPropertyName("signup_time")]
		public DateTimeOffset? SignupTime { get; set; }

		/// <summary>
		/// The date and time the user's most recent view.
		/// </summary>
		[JsonPropertyName("view_time")]
		public DateTimeOffset? ViewTime { get; set; }
	}

	/// <summary>
	/// Represents information about a user's devices.
	/// </summary>
	public class SailthruUserDevice
	{
		/// <summary>
		/// Gets the top device for reading emails.
		/// </summary>
		[JsonPropertyName("top_device_email")]
		public string? Email { get; set; }
	}

	/// <summary>
	/// Represents lifetime information about the user's subscription.
	/// </summary>
	public class SailthruUserLifetime
	{
		/// <summary>
		/// Gets the number of messages.
		/// </summary>
		[JsonPropertyName("lifetime_message")]
		public int Messages { get; set; }

		/// <summary>
		/// Gets the number of page views.
		/// </summary>
		[JsonPropertyName("lifetime_pv")]
		public int PageViews { get; set; }

		/// <summary>
		/// Gets the number of messages opened.
		/// </summary>
		[JsonPropertyName("lifetime_open")]
		public int Opens { get; set; }

		/// <summary>
		/// Gets the number of purchases.
		/// </summary>
		[JsonPropertyName("lifetime_purchase")]
		public int Purchases { get; set; }

		/// <summary>
		/// Gets the total purchase price of all of the user's purchases.
		/// </summary>
		[JsonPropertyName("lifetime_purchase_price"), JsonConverter(typeof(PriceConverter))]
		public decimal TotalPurchasePrice { get; set; }
	}

	/// <summary>
	/// Represents a Sailthru purchase.
	/// </summary>
	public class SailthruUserPurchase
	{
		/// <summary>
		/// Gets the price of the item.
		/// </summary>
		[JsonPropertyName("price"), JsonConverter(typeof(PriceConverter))]
		public decimal Price { get; set; }

		/// <summary>
		/// Gets the quantity.
		/// </summary>
		[JsonPropertyName("qty")]
		public int Quantity { get; set; }

		/// <summary>
		/// Gets the time.
		/// </summary>
		[JsonPropertyName("time")]
		public DateTimeOffset Time { get; set; }

		/// <summary>
		/// Gets the set of items.
		/// </summary>
		[JsonPropertyName("items")]
		public SailthruUserPurchaseItem[] Items { get; set; } = default!;
	}

	/// <summary>
	/// Represents a sailthru purchase item.
	/// </summary>
	public class SailthruUserPurchaseItem
	{
		/// <summary>
		/// Gets the title of the purchase.
		/// </summary>
		[JsonPropertyName("title")]
		public string? Title { get; set; }

		/// <summary>
		/// Gets the unique item ID.
		/// </summary>
		[JsonPropertyName("id")]
		public string Id { get; set; } = default!;

		/// <summary>
		/// Gets the URL of the item that was purchased.
		/// </summary>
		[JsonPropertyName("url")]
		public string? Url { get; set; }

		/// <summary>
		/// Gets the price of the item.
		/// </summary>
		[JsonPropertyName("price"), JsonConverter(typeof(PriceConverter))]
		public decimal Price { get; set; }

		/// <summary>
		/// Gets the quantity.
		/// </summary>
		[JsonPropertyName("qty")]
		public int Quantity { get; set; }

		/// <summary>
		/// Gets the tags.
		/// </summary>
		[JsonPropertyName("tags")]
		public string[]? Tags { get; set; }

		/// <summary>
		/// Gets or sets the set of variables associated with the purchase item.
		/// </summary>
		[JsonPropertyName("vars")]
		public Map<string>? Vars { get; set; }
	}

	/// <summary>
	/// Defines the possbile Sailthru user key types.
	/// </summary>
	public sealed class SailthruUserKeyType
	{
		public const string Cookie = "cookie";
		public const string Email = "email";
		public const string ExternalId = "exid";
		public const string Facebook = "fb";
		public const string SailthruId = "sid";
		public const string Sms = "sms";
		public const string Twitter = "twitter";
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
		public bool OptOutStatus;
		public int PurchaseIncomplete = 0;
		public int Purchases = 0;
		public bool SmartLists;
		public bool Vars;
	}
}

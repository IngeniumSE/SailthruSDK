namespace SailthruSDK.Api
{
	using System;
	using System.Text.Json;
	using System.Text.Json.Serialization;
	using SailthruSDK;
	using SailthruSDK.Converters;

	/// <summary>
	/// Represents a Sailthru user.
	/// </summary>
	public class User
	{
		/// <summary>
		/// Gets recent user activity.
		/// </summary>
		[JsonPropertyName("activity")]
		public UserActivity? Activity { get; set; }

		/// <summary>
		/// Gets or sets the device.
		/// </summary>
		[JsonPropertyName("device")]
		public UserDevice? Device { get; set; }

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
		public UserLifetime? Lifetime { get; set; }

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
		public Purchase[]? Purchases { get; set; }

		/// <summary>
		/// Gets the set of purchases.
		/// </summary>
		[JsonPropertyName("purchase_incomplete")]
		public Purchase[]? IncompletePurchases { get; set; }

		/// <summary>
		/// Gets or sets the set of variables associated with the user.
		/// </summary>
		[JsonPropertyName("vars")]
		public Map<string?>? Vars { get; set; }
	}

	/// <summary>
	/// Represents sailthru user activity.
	/// </summary>
	public class UserActivity
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
	public class UserDevice
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
	public class UserLifetime
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
	/// Defines the possbile Sailthru user key types.
	/// </summary>
	public sealed class UserKeyType
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
	public class UserFields
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

	/// <summary>
	/// Represents a request to get a user.
	/// </summary>
	public class GetUserRequest(
		string id,
		string key = UserKeyType.Email,
		UserFields? fields = default)
	{
		/// <summary>
		/// Gets the user ID.
		/// </summary>
		public string Id { get; } = id;

		/// <summary>
		/// Gets the key type.
		/// </summary>
		public string Key { get; } = key;

		/// <summary>
		/// Gets the set of fields.
		/// </summary>
		public Map<OneOf<bool, int>>? Fields { get; } = ToMap(fields);

		static Map<OneOf<bool, int>>? ToMap(UserFields? fields)
		{
			if (fields is null)
			{
				return default;
			}

			var map = new Map<OneOf<bool, int>>();
			Map(map, "activity", fields.Activity);
			Map(map, "device", fields.Device);
			Map(map, "engagement", fields.Engagement);
			Map(map, "keys", fields.Keys);
			Map(map, "lifetime", fields.Lifetime);
			Map(map, "lists", fields.Lists);
			Map(map, "optout_email", fields.OptOutStatus);
			Map(map, "purchase_incomplete", fields.PurchaseIncomplete);
			Map(map, "purchases", fields.Purchases);
			Map(map, "smart_lists", fields.SmartLists);
			Map(map, "vars", fields.Vars);

			return map;
		}

		static void Map(Map<OneOf<bool, int>> map, string name, bool value)
		{
			if (value)
			{
				map[name] = new OneOf<bool, int>(value);
			}
		}

		static void Map(Map<OneOf<bool, int>> map, string name, int value)
		{
			if (value > 0)
			{
				map[name] = new OneOf<bool, int>(value);
			}
		}

		/// <summary>
		/// Provides custom serialization of a <see cref="GetUserRequest"/>
		/// </summary>
		internal class Converter : ConverterBase<GetUserRequest>
		{
			/// <inhertdoc />
			public override GetUserRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				throw new NotImplementedException();
			}

			/// <inhertdoc />
			public override void Write(Utf8JsonWriter writer, GetUserRequest value, JsonSerializerOptions options)
			{
				if (value is null)
				{
					writer.WriteNullValue();
				}
				else
				{
					writer.WriteStartObject();

					writer.WriteStringProperty("id", value.Id, options);
					writer.WriteStringProperty("key", value.Key, options);
					writer.WriteUserFields(value.Fields, options);

					writer.WriteEndObject();
				}
			}
		}
	}

	public class UpsertUserRequest(
		string id,
		string key = UserKeyType.Email,
		Map<string>? keys = default,
		KeyConflict keyConflict = KeyConflict.Merge,
		Map<string>? cookies = default,
		Map<bool>? lists = default,
		Map<bool>? templates = default,
		Map<string?>? vars = default,
		OptOutStatus? optOutEmailStatus = default,
		bool? optOutSms = default,
		UserFields? fields = default)
	{

		/// <summary>
		/// Gets the user ID.
		/// </summary>
		public string Id { get; } = Ensure.IsNotNullOrEmpty(id, nameof(id));	

		/// <summary>
		/// Gets the key type.
		/// </summary>
		public string Key { get; } = Ensure.IsNotNullOrEmpty(key, nameof(key));

		/// <summary>
		/// Gets the key conflict option.
		/// </summary>
		public KeyConflict KeyConflict { get; } = keyConflict;

		/// <summary>
		/// Gets the set of cookies.
		/// </summary>
		public Map<string>? Cookies { get; } = cookies;

		/// <summary>
		/// Gets the set of alternate keys for the user.
		/// </summary>
		public Map<string>? Keys { get; } = keys;

		/// <summary>
		/// Gets he set of list registrations.
		/// </summary>
		public Map<bool>? Lists { get; } = lists;

		/// <summary>
		/// Gets the opt-out status for emails.
		/// </summary>
		public OptOutStatus? OptOutEmailStatus { get; } = optOutEmailStatus;

		/// <summary>
		/// Gets the out-out status for SMS.
		/// </summary>
		public bool? OptOutSmsStatus { get; } = optOutSms;

		/// <summary>
		/// Gets the set of template opt-outs.
		/// </summary>
		public Map<bool>? Templates { get; } = templates;

		/// <summary>
		/// Gets the set of variables.
		/// </summary>
		public Map<string?>? Vars { get; } = vars;

		/// <summary>
		/// Gets the set of fields.
		/// </summary>
		public Map<OneOf<bool, int>>? Fields { get; } = ToMap(fields);

		static Map<OneOf<bool, int>>? ToMap(UserFields? fields)
		{
			if (fields is null)
			{
				return default;
			}

			var map = new Map<OneOf<bool, int>>();
			Map(map, "activity", fields.Activity);
			Map(map, "device", fields.Device);
			Map(map, "engagement", fields.Engagement);
			Map(map, "keys", fields.Keys);
			Map(map, "lifetime", fields.Lifetime);
			Map(map, "lists", fields.Lists);
			Map(map, "optout_email", fields.OptOutStatus);
			Map(map, "purchase_incomplete", fields.PurchaseIncomplete);
			Map(map, "purchases", fields.Purchases);
			Map(map, "smart_lists", fields.SmartLists);
			Map(map, "vars", fields.Vars);

			return map;
		}

		static void Map(Map<OneOf<bool, int>> map, string name, bool value)
		{
			if (value)
			{
				map[name] = new OneOf<bool, int>(value);
			}
		}

		static void Map(Map<OneOf<bool, int>> map, string name, int value)
		{
			if (value > 0)
			{
				map[name] = new OneOf<bool, int>(value);
			}
		}

		/// <summary>
		/// Provides custom serialization of a <see cref="UpsertUserRequest"/>
		/// </summary>
		internal class Converter : ConverterBase<UpsertUserRequest>
		{
			/// <inhertdoc />
			public override UpsertUserRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				throw new NotImplementedException();
			}

			/// <inhertdoc />
			public override void Write(Utf8JsonWriter writer, UpsertUserRequest value, JsonSerializerOptions options)
			{
				if (value is null)
				{
					writer.WriteNullValue();
				}
				else
				{
					writer.WriteStartObject();

					writer.WriteStringProperty("id", value.Id, options);
					writer.WriteStringProperty("key", value.Key, options);
					writer.WriteMapProperty("keys", value.Keys, options);
					writer.WriteEnumProperty<KeyConflict>("keyconflict", value.KeyConflict, options);
					writer.WriteMapProperty("cookies", value.Cookies, options);
					writer.WriteMapProperty("lists", value.Lists, options);
					writer.WriteMapProperty("optout_templates", value.Templates, options);
					writer.WriteMapProperty("vars", value.Vars, options);
					writer.WriteEnumProperty<OptOutStatus>("optout_email", value.OptOutEmailStatus, options);
					if (value.OptOutSmsStatus.GetValueOrDefault(false))
					{
						writer.WriteStringProperty("optout_sms_status", "opt-out", options);
					}

					writer.WriteUserFields(value.Fields, options);

					writer.WriteEndObject();
				}
			}
		}
	}
}

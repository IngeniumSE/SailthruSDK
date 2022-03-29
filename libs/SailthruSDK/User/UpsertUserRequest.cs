namespace SailthruSDK.User
{
	using System;
	using System.Text.Json;

	using SailthruSDK.Converters;

	/// <summary>
	/// Represents a request to create or update a user.
	/// </summary>
	public class UpsertUserRequest
	{
		/// <summary>
		/// Initialises a new instance of <see cref="GetUserRequest"/>
		/// </summary>
		/// <param name="id">The user ID.</param>
		/// <param name="key">The key type.</param>
		/// <param name="fields">The set of fields to return.</param>
		public UpsertUserRequest(
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
			SailthruUserFields? fields = default)
		{
			Id = Ensure.IsNotNull(id, nameof(id));
			Key = Ensure.IsNotNullOrEmpty(key, nameof(key));
			KeyConflict = keyConflict;
			Keys = keys;
			Cookies = cookies;
			Lists = lists;
			Templates = templates;
			Vars = vars;
			OptOutEmailStatus = optOutEmailStatus;
			OptOutSmsStatus = optOutSms;
			Fields = ToMap(fields);
		}

		/// <summary>
		/// Gets the user ID.
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the key type.
		/// </summary>
		public string Key { get; }

		/// <summary>
		/// Gets the key conflict option.
		/// </summary>
		public KeyConflict KeyConflict { get; }

		/// <summary>
		/// Gets the set of cookies.
		/// </summary>
		public Map<string>? Cookies { get; }

		/// <summary>
		/// Gets the set of alternate keys for the user.
		/// </summary>
		public Map<string>? Keys { get; }

		/// <summary>
		/// Gets he set of list registrations.
		/// </summary>
		public Map<bool>? Lists { get; }

		/// <summary>
		/// Gets the opt-out status for emails.
		/// </summary>
		public OptOutStatus? OptOutEmailStatus { get; }

		/// <summary>
		/// Gets the out-out status for SMS.
		/// </summary>
		public bool? OptOutSmsStatus { get; }

		/// <summary>
		/// Gets the set of template opt-outs.
		/// </summary>
		public Map<bool>? Templates { get; }

		/// <summary>
		/// Gets the set of variables.
		/// </summary>
		public Map<string>? Vars { get; }

		/// <summary>
		/// Gets the set of fields.
		/// </summary>
		public Map<OneOf<bool, int>>? Fields { get; }

		Map<OneOf<bool, int>>? ToMap(SailthruUserFields? fields)
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

		void Map(Map<OneOf<bool, int>> map, string name, bool value)
		{
			if (value)
			{
				map[name] = new OneOf<bool, int>(value);
			}
		}

		void Map(Map<OneOf<bool, int>> map, string name, int value)
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

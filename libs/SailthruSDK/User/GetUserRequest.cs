namespace SailthruSDK.User
{
	using System;
	using System.Text.Json;
	using System.Text.Json.Serialization;

	using SailthruSDK.Converters;

	/// <summary>
	/// Represenst a request to get a user.
	/// </summary>
	public class GetUserRequest
	{
		/// <summary>
		/// Initialises a new instance of <see cref="GetUserRequest"/>
		/// </summary>
		/// <param name="id">The user ID.</param>
		/// <param name="key">The key type.</param>
		/// <param name="fields">The set of fields to return.</param>
		public GetUserRequest(
			string id,
			string key = SailthruUserKeyType.Email,
			SailthruUserFields? fields = default)
		{
			Id = Ensure.IsNotNull(id, nameof(id));
			Key = Ensure.IsNotNullOrEmpty(key, nameof(key));
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
}

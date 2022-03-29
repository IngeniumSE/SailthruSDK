namespace SailthruSDK
{
	using System;
	using System.Text.Json;
	using System.Text.Json.Serialization;

	/// <summary>
	/// Provides extensions for the <see cref="Utf8JsonWriter"/> type.
	/// </summary>
	public static class JsonWriterExtensions
	{
		public static void WriteUserFields(
			this Utf8JsonWriter writer,
			Map<OneOf<bool, int>>? fields,
			JsonSerializerOptions options)
		{
			if (writer is null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			if (fields is { Count: > 0 })
			{
				writer.WritePropertyName("fields");
				var converter = options.GetConverter(typeof(Map<OneOf<bool, int>>)) as JsonConverter<Map<OneOf<bool, int>>>;
				if (converter is not null)
				{
					converter.Write(writer, fields, options);
				}
				else
				{
					JsonSerializer.Serialize(writer, fields, options);
				}
			}
		}

		public static void WriteStringProperty(
			this Utf8JsonWriter writer,
			string key,
			string? value,
			JsonSerializerOptions options)
		{
			if (writer is null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			writer.WritePropertyName(GetName(key, options));

			if (value is null)
			{
				writer.WriteNullValue();
			}
			else
			{
				writer.WriteStringValue(value);
			}
		}

		public static void WriteNumberProperty(
			this Utf8JsonWriter writer,
			string key,
			int value,
			JsonSerializerOptions options)
		{
			if (writer is null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			writer.WritePropertyName(GetName(key, options));
			writer.WriteNumberValue(value);
		}

		public static void WriteBooleanProperty(
			this Utf8JsonWriter writer,
			string key,
			bool value,
			JsonSerializerOptions options)
		{
			if (writer is null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			writer.WritePropertyName(GetName(key, options));
			writer.WriteBooleanValue(value);
		}

		public static void WriteMapProperty<TValue>(
			this Utf8JsonWriter writer,
			string key,
			Map<TValue>? map,
			JsonSerializerOptions options)
		{
			if (writer is null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			if (map is not { Count: > 0 })
			{
				return;
			}

			writer.WritePropertyName(GetName(key, options));

			writer.WriteStartObject();

			foreach (var pair in map)
			{
				writer.WritePropertyName(pair.Key);

				JsonSerializer.Serialize(writer, pair.Value, options);
			}

			writer.WriteEndObject();
		}

		public static void WriteEnumProperty<TEnum>(
			this Utf8JsonWriter writer,
			string key,
			TEnum? value,
			JsonSerializerOptions options)
			where TEnum : struct
		{
			if (writer is null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			if (!value.HasValue)
			{
				return;
			}

			writer.WritePropertyName(GetName(key, options));
			writer.WriteStringValue(value.ToString().ToLower());
		}

		static string GetName(string name, JsonSerializerOptions options)
			=> options.PropertyNamingPolicy!.ConvertName(name) ?? name;
	}
}

namespace SailthruSDK.Converters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.Json;
	using System.Text.Json.Serialization;

	/// <summary>
	/// Deserializes a <see cref="Map{TValue}"/> type.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	internal class MapConverter<T> : ConverterBase<Map<T>>
	{
		public override Map<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var map = new Map<T>();
			var source = JsonSerializer.Deserialize<Dictionary<string, T>>(ref reader, options);
			if (source is { Count: >0 })
			{
				foreach (var pair in source)
				{
					map.Add(pair.Key, pair.Value);
				}
			}

			return map;
		}

		public override void Write(Utf8JsonWriter writer, Map<T> value, JsonSerializerOptions options)
		{
			if (value is null)
			{
				writer.WriteNullValue();
			}
			else
			{
				writer.WriteStartObject();
				var converter = options.GetConverter(typeof(T)) as JsonConverter<T>;

				if (value.Count > 0)
				{
					foreach (var pair in value.OrderBy(p => p.Key, StringComparer.Ordinal))
					{
						writer.WritePropertyName(GetName(pair.Key, options));

						if (converter is null)
						{
							JsonSerializer.Serialize(writer, pair.Value, options);
						}
						else
						{
							converter.Write(writer, pair.Value, options);
						}
					}
				}

				writer.WriteEndObject();
			}
		}
	}
}

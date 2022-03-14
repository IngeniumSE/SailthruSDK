namespace SailthruSDK.Converters
{
	using System;
	using System.Text.Json;

	/// <summary>
	/// Represents a converter for handling nullable enum values.
	/// </summary>
	/// <typeparam name="TEnum">The enum type.</typeparam>
	internal class NullableEnumConverter<TEnum> : ConverterBase<TEnum?>
		where TEnum : struct
	{
		/// <inheritdoc />
		public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			string? value = reader.GetString();
			if (value is { Length: > 0 })
			{
				if (Enum.TryParse<TEnum>(value, true, out var enumValue))
				{
					return enumValue;
				}

				return default;
			}
			else
			{
				return default;
			}
		}

		/// <inheritdoc />
		public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
		{
			if (value.HasValue)
			{
				writer.WriteStringValue(value.Value.ToString());
			}
			else
			{
				writer.WriteNullValue();
			}
		}
	}
}

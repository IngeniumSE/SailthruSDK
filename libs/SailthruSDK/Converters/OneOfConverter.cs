namespace SailthruSDK.Converters
{
	using System;
	using System.Text.Json;
	using System.Text.Json.Serialization;

	/// <summary>
	/// Serializes a <see cref="OneOf{TFirst, TSecond}"/> type.
	/// </summary>
	/// <typeparam name="TFirst">The first type.</typeparam>
	/// <typeparam name="TSecond">The second type.</typeparam>
	internal class OneOfConverter<TFirst, TSecond> : ConverterBase<OneOf<TFirst, TSecond>>
	{
		public override OneOf<TFirst, TSecond> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			=> default;

		public override void Write(Utf8JsonWriter writer, OneOf<TFirst, TSecond> value, JsonSerializerOptions options)
		{
			if (value.HasValue)
			{
				if (value.IsFirst)
				{
					Write(writer, value.First, options);
				}
				else
				{
					Write(writer, value.Second, options);
				}
			}
			else
			{
				writer.WriteNullValue();
			}
		}

		static void Write<TValue>(Utf8JsonWriter writer, TValue value, JsonSerializerOptions options)
		{
			var converter = options.GetConverter(typeof(TValue)) as JsonConverter<TValue>;
			if (converter is not null)
			{
				converter.Write(writer, value, options);
			}
			else
			{
				JsonSerializer.Serialize(writer, value, options);
			}
		}
	}
}

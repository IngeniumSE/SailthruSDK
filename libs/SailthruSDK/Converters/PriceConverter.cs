namespace SailthruSDK.Converters
{
	using System;
	using System.Text.Json;

	/// <summary>
	/// Provides a conversion of a price field store as whole integers into a decimal.
	/// </summary>
	internal class PriceConverter : ConverterBase<decimal>
	{
		/// <inheritdoc />
		public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			int value = reader.GetInt32();
			return ((decimal)value) / 100;
		}

		/// <inheritdoc />
		public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) { }
	}
}

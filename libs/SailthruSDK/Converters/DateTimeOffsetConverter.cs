namespace SailthruSDK.Converters
{
	using System;
	using System.Globalization;
	using System.Text.Json;

	/// <summary>
	/// Provides conversion of <see cref="DateTimeOffset"/> values using the format used by Sailthru.
	/// </summary>
	internal class NullableDateTimeOffsetConverter : ConverterBase<DateTimeOffset?>
	{
		const string DateTimeFormat = "ddd, dd MMM yyyy HH:mm:ss";
		const string TimeZoneFormat = "hhmm";

		/// <inheritdoc />
		public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			string? value = reader.GetString();
			if (value is { Length: > 0 })
			{
				string suffix = value.Substring(value.Length - 5, 5).Trim();
				value = value.Substring(0, value.Length - 5).Trim();

				bool isNegative = suffix.StartsWith("-");
				suffix = suffix.TrimStart('-', '+');

				if (TimeSpan.TryParseExact(suffix, TimeZoneFormat, CultureInfo.InvariantCulture, out var timeZone))
				{
					if (isNegative)
					{
						timeZone = -timeZone;
					}
				}
				else
				{
					return default;
				}

				if (DateTime.TryParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateTime))
				{
					return new DateTimeOffset(dateTime, timeZone);
				}

				return default;
			}

			return default;
		}

		/// <inheritdoc />
		public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options) { }
	}

	/// <summary>
	/// Provides conversion of <see cref="DateTimeOffset"/> values using the format used by Sailthru.
	/// </summary>
	internal class DateTimeOffsetConverter : ConverterBase<DateTimeOffset>
	{
		const string DateTimeFormat = "ddd, dd MMM yyyy HH:mm:ss";
		const string TimeZoneFormat = "hhmm";

		/// <inheritdoc />
		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			string? value = reader.GetString();
			if (value is { Length: > 0 })
			{
				string suffix = value.Substring(value.Length - 5, 5).Trim();
				value = value.Substring(0, value.Length - 5).Trim();

				bool isNegative = suffix.StartsWith("-");
				suffix = suffix.TrimStart('-', '+');

				if (TimeSpan.TryParseExact(suffix, TimeZoneFormat, CultureInfo.InvariantCulture, out var timeZone))
				{
					if (isNegative)
					{
						timeZone = -timeZone;
					}
				}
				else
				{
					return default;
				}

				if (DateTime.TryParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateTime))
				{
					return new DateTimeOffset(dateTime, timeZone);
				}

				return default;
			}

			return default;
		}

		/// <inheritdoc />
		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) { }
	}
}

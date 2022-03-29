namespace SailthruSDK
{
	using System;
	using System.Text.Json;
	using System.Text.Json.Serialization;

	using SailthruSDK.Converters;
	using SailthruSDK.Purchase;
	using SailthruSDK.User;

	static class JsonUtility
	{
		public static JsonSerializerOptions GetSerializerOptions()
		{
			var options = new JsonSerializerOptions()
			{
				WriteIndented = false,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			/** MODELS **/
			options.Converters.Add(new GetUserRequest.Converter());
			options.Converters.Add(new UpsertUserRequest.Converter());
			options.Converters.Add(new UpsertPurchaseRequest.Convereter());	
			options.Converters.Add(new DateTimeOffsetConverter());
			options.Converters.Add(new NullableDateTimeOffsetConverter());
			options.Converters.Add(new OneOfConverter<bool, int>());
			options.Converters.Add(new NullableEnumConverter<OptOutStatus>());

			/** MAP  **/
			options.Converters.Add(new MapConverter<bool>());
			options.Converters.Add(new MapConverter<string>());

			return options;
		}
	}
}

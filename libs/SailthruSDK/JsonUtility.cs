namespace SailthruSDK;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using SailthruSDK.Api;
using SailthruSDK.Converters;
static class JsonUtility
{
	public static JsonSerializerOptions CreateSerializerOptions()
	{
		var options = new JsonSerializerOptions()
		{
			WriteIndented = false,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		/** MODELS **/
		options.Converters.Add(new GetUserRequest.Converter());
		options.Converters.Add(new UpsertUserRequest.Converter());
		options.Converters.Add(new UpsertPurchaseRequest.Converter());	
		options.Converters.Add(new DateTimeOffsetConverter());
		options.Converters.Add(new NullableDateTimeOffsetConverter());
		options.Converters.Add(new OneOfConverter<bool, int>());
		options.Converters.Add(new NullableEnumConverter<OptOutStatus>());

		/** MAP  **/
		options.Converters.Add(new StringMapConverter());
		options.Converters.Add(new MapConverter<bool>());

		return options;
	}
}

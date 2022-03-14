namespace SailthruSDK.Converters
{
using System.Text.Json;
	using System.Text.Json.Serialization;

	/// <summary>
	/// Provides a base implementation fo a converter.
	/// </summary>
	/// <typeparam name="T">The model type.</typeparam>
	internal abstract class ConverterBase<T> : JsonConverter<T>
	{
		protected string GetName(string name, JsonSerializerOptions options)
			=> options.PropertyNamingPolicy!.ConvertName(name);
	}
}

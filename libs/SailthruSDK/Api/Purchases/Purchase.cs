// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Text.Json;
using System.Text.Json.Serialization;

using SailthruSDK.Converters;

namespace SailthruSDK.Api;

/// <summary>
/// Represents a Sailthru purchase.
/// </summary>
public class Purchase
{
	/// <summary>
	/// Gets the price of the item.
	/// </summary>
	public int Price { get; set; }

	/// <summary>
	/// Gets the quantity.
	/// </summary>
	[JsonPropertyName("qty")]
	public int Quantity { get; set; }

	/// <summary>
	/// Gets the time.
	/// </summary>
	[JsonPropertyName("time")]
	public DateTimeOffset Time { get; set; }

	/// <summary>
	/// Gets the set of items.
	/// </summary>
	[JsonPropertyName("items")]
	public PurchaseItem[] Items { get; set; } = default!;
}

/// <summary>
/// Represents a sailthru purchase item.
/// </summary>
public class PurchaseItem
{
	/// <summary>
	/// Gets the title of the purchase.
	/// </summary>
	[JsonPropertyName("title")]
	public string? Title { get; set; }

	/// <summary>
	/// Gets the unique item ID.
	/// </summary>
	[JsonPropertyName("id")]
	public string Id { get; set; } = default!;

	/// <summary>
	/// Gets the URL of the item that was purchased.
	/// </summary>
	[JsonPropertyName("url")]
	public string? Url { get; set; }

	/// <summary>
	/// Gets the price of the item.
	/// </summary>
	public int Price { get; set; }

	/// <summary>
	/// Gets the quantity.
	/// </summary>
	[JsonPropertyName("qty")]
	public int Quantity { get; set; }

	/// <summary>
	/// Gets the tags.
	/// </summary>
	[JsonPropertyName("tags")]
	public string[]? Tags { get; set; }

	/// <summary>
	/// Gets or sets the set of variables associated with the purchase item.
	/// </summary>
	[JsonPropertyName("vars")]
	public Map<string?>? Vars { get; set; }

	/// <summary>
	/// Gets or sets the set of images.
	/// </summary>
	[JsonPropertyName("images")]
	public PurchaseImage[]? Images { get; set; }
}

/// <summary>
/// Represents a purchase image.
/// </summary>
public class PurchaseImage
{
	[JsonPropertyName("full")]
	public PurchaseImageUrl? Full { get; set; }

	[JsonPropertyName("thumb")]
	public PurchaseImageUrl? Thumb { get; set; }
}

/// <summary>
/// Represents a URL container.
/// </summary>
public class PurchaseImageUrl
{
	[JsonPropertyName("url")]
	public string Url { get; set; } = null!;
}

/// <summary>
/// Represents a request to create or update a purchase
/// </summary>
public class UpsertPurchaseRequest
{
	/// <summary>
	/// Initialises a new instance of <see cref="UpsertPurchaseRequest"/>
	/// </summary>
	/// <param name="email">The user email address</param>
	/// <param name="items">The set of items.</param>
	/// <param name="incomplete">Specifies whether the purchase is incomplete (e.g. an active cart, not an order)</param>
	/// <param name="messageId">The message ID representing the email campaign. This is usually stored in the sailthru_bid cookie.</param>
	public UpsertPurchaseRequest(
		string email,
		PurchaseItem[] items,
		bool incomplete = false,
		string? messageId = default)
	{
		Email = Ensure.IsNotNullOrEmpty(email, nameof(email));
		Items = Ensure.IsNotNull(items, nameof(items));
		Incomplete = incomplete;
		MessageId = messageId;
	}

	/// <summary>
	/// Gets the email address.
	/// </summary>
	public string Email { get; }

	/// <summary>
	/// Gets whether the purchase is incomplete.
	/// </summary>
	public bool Incomplete { get; }

	/// <summary>
	/// Gets the set of purchase items.
	/// </summary>
	public PurchaseItem[] Items { get; }

	/// <summary>
	/// Gets the message campaign ID. This is usually stored in the sailthru_bid cookie.
	/// </summary>
	public string? MessageId { get; }

	internal class Converter : ConverterBase<UpsertPurchaseRequest>
	{
		/// <inhertdoc />
		public override UpsertPurchaseRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}

		/// <inhertdoc />
		public override void Write(Utf8JsonWriter writer, UpsertPurchaseRequest value, JsonSerializerOptions options)
		{
			if (value is null)
			{
				writer.WriteNullValue();
			}
			else
			{
				writer.WriteStartObject();

				writer.WriteStringProperty("email", value.Email, options);
				writer.WriteBooleanProperty("incomplete", value.Incomplete, options);
				writer.WriteStringProperty("message_id", value.MessageId ?? default, options);

				writer.WritePropertyName("items");
				writer.WriteStartArray();

				foreach (var item in value.Items)
				{
					writer.WriteStartObject();

					writer.WriteStringProperty("id", item.Id, options);
					writer.WriteStringProperty("title", item.Title, options);
					writer.WriteNumberProperty("price", item.Price, options);
					writer.WriteNumberProperty("qty", item.Quantity, options);
					writer.WriteStringProperty("url", item.Url, options);

					if (item.Images is { Length: > 0 })
					{
						writer.WritePropertyName("images");
						writer.WriteStartArray();

						foreach (var image in item.Images)
						{
							writer.WriteStartObject();

							if (image.Full != null)
							{
								writer.WritePropertyName("full");
								writer.WriteStartObject();
								writer.WriteStringProperty("url", image.Full.Url, options);
								writer.WriteEndObject();
							}

							if (image.Thumb != null)
							{
								writer.WritePropertyName("thumb");
								writer.WriteStartObject();
								writer.WriteStringProperty("url", image.Thumb.Url, options);
								writer.WriteEndObject();
							}

							writer.WriteEndObject();
						}

						writer.WriteEndArray();
					}

					writer.WriteEndObject();
				}

				writer.WriteEndArray();
				writer.WriteEndObject();
			}
		}
	}
}

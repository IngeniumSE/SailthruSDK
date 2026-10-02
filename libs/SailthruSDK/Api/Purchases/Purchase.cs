// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using System.Globalization;
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
	/// Gets or sets the set of images. Sailthru accepts one full and one thumbnail image per item; see <see cref="PurchaseImage"/>.
	/// </summary>
	[JsonPropertyName("images")]
	public PurchaseImage[]? Images { get; set; }
}

/// <summary>
/// Represents a purchase image.
/// </summary>
/// <remarks>
/// Sailthru stores one full-size and one thumbnail image per item, sent as
/// <c>"images": { "full": { "url": "..." }, "thumb": { "url": "..." } }</c>.
/// When <see cref="PurchaseItem.Images"/> has more than one entry, the first non-empty full and thumb URLs are used.
/// </remarks>
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
	/// The format used to send <see cref="Date"/>: ISO 8601 in UTC, e.g. <c>2026-10-01T09:30:00+00:00</c>.
	/// </summary>
	public const string DateFormat = "yyyy-MM-dd'T'HH:mm:ss'+00:00'";

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

	/// <summary>
	/// Gets or sets your own unique ID for the purchase (e.g. the order reference), sent as
	/// <c>purchase_keys.extid</c>. Sailthru rejects a second purchase with the same extid
	/// ("Duplicate extid", see <see cref="Error.IsDuplicateExtId"/>), which makes retries safe.
	/// Not allowed on an incomplete purchase.
	/// </summary>
	public string? ExtId { get; set; }

	/// <summary>
	/// Gets or sets the date and time of the purchase. Sailthru defaults to the time it receives the purchase;
	/// set this when sending a purchase late (retries, backfill). Sent in UTC, see <see cref="DateFormat"/>.
	/// </summary>
	public DateTimeOffset? Date { get; set; }

	/// <summary>
	/// Gets or sets custom order-level variables.
	/// </summary>
	public Map<string?>? Vars { get; set; }

	/// <summary>
	/// Validates the request against Sailthru's rules.
	/// </summary>
	/// <exception cref="ArgumentException">An incomplete purchase has an <see cref="ExtId"/>.</exception>
	public void Validate()
	{
		if (Incomplete && ExtId is { Length: > 0 })
		{
			throw new ArgumentException(Resources.UpsertPurchaseRequest_IncompleteWithExtId, nameof(ExtId));
		}
	}

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
				writer.WriteStringProperty("message_id", value.MessageId is { Length: > 0 } ? value.MessageId : default, options);

				if (value.ExtId is { Length: > 0 })
				{
					writer.WritePropertyName("purchase_keys");
					writer.WriteStartObject();
					writer.WriteStringProperty("extid", value.ExtId, options);
					writer.WriteEndObject();
				}

				if (value.Date.HasValue)
				{
					writer.WriteStringProperty(
						"date",
						value.Date.Value.ToUniversalTime().ToString(DateFormat, CultureInfo.InvariantCulture),
						options);
				}

				WriteVars(writer, value.Vars);

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

					if (item.Tags is { Length: > 0 })
					{
						writer.WritePropertyName("tags");
						writer.WriteStartArray();
						foreach (var tag in item.Tags)
						{
							writer.WriteStringValue(tag);
						}
						writer.WriteEndArray();
					}

					WriteVars(writer, item.Vars);

					WriteImages(writer, item.Images, options);

					writer.WriteEndObject();
				}

				writer.WriteEndArray();
				writer.WriteEndObject();
			}
		}

		static void WriteVars(Utf8JsonWriter writer, Map<string?>? vars)
		{
			if (vars is { Count: > 0 })
			{
				// Var names are the caller's own keys: written as given, not through the naming policy.
				writer.WritePropertyName("vars");
				writer.WriteStartObject();
				foreach (var pair in vars)
				{
					writer.WritePropertyName(pair.Key);
					if (pair.Value is null)
					{
						writer.WriteNullValue();
					}
					else
					{
						writer.WriteStringValue(pair.Value);
					}
				}
				writer.WriteEndObject();
			}
		}

		static void WriteImages(Utf8JsonWriter writer, PurchaseImage[]? images, JsonSerializerOptions options)
		{
			if (images is not { Length: > 0 })
			{
				return;
			}

			// Sailthru's documented shape is an object, not an array: { "full": { "url" }, "thumb": { "url" } }.
			var full = images.Select(i => i?.Full?.Url).FirstOrDefault(u => u is { Length: > 0 });
			var thumb = images.Select(i => i?.Thumb?.Url).FirstOrDefault(u => u is { Length: > 0 });

			if (full is null && thumb is null)
			{
				return;
			}

			writer.WritePropertyName("images");
			writer.WriteStartObject();

			if (full is not null)
			{
				writer.WritePropertyName("full");
				writer.WriteStartObject();
				writer.WriteStringProperty("url", full, options);
				writer.WriteEndObject();
			}

			if (thumb is not null)
			{
				writer.WritePropertyName("thumb");
				writer.WriteStartObject();
				writer.WriteStringProperty("url", thumb, options);
				writer.WriteEndObject();
			}

			writer.WriteEndObject();
		}
	}
}

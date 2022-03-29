using System;
using System.Text.Json;

using SailthruSDK.Converters;

namespace SailthruSDK.Purchase
{
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
		public UpsertPurchaseRequest(
			string email,
			PurchaseItem[] items,
			bool incomplete = false)
		{
			Email = Ensure.IsNotNullOrEmpty(email, nameof(email));
			Items = Ensure.IsNotNull(items, nameof(items));
			Incomplete = incomplete;
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

		internal class Convereter : ConverterBase<UpsertPurchaseRequest>
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

						writer.WriteEndObject();
					}

					writer.WriteEndArray();
					writer.WriteEndObject();
				}
			}
		}
	}

	/// <summary>
	/// Represents a purchase item.
	/// </summary>
	public class PurchaseItem
	{
		public PurchaseItem(
			string id,
			string title,
			int price,
			int quantity,
			string url)
		{
			Id = Ensure.IsNotNullOrEmpty(id, nameof(id));
			Title = Ensure.IsNotNullOrEmpty(title, nameof(title));
			Price = price;
			Quantity = quantity;
			Url = Ensure.IsNotNullOrEmpty(url, nameof(url));
		}

		/// <summary>
		/// Gets the ID.
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the item title.
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Gets the price.
		/// </summary>
		public int Price { get; }

		/// <summary>
		/// Gets the quantity.
		/// </summary>
		public int Quantity { get; }

		/// <summary>
		/// Gets the URL.
		/// </summary>
		public string Url { get; }
	}
}
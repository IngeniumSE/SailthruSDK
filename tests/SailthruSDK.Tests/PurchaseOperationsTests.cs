namespace SailthruSDK.Tests
{
	using System.Net;
	using System.Text.Json;

	using SailthruSDK.Api;

	using Xunit;

	/// <summary>
	/// Provides tests for the purchase operations and Sailthru error handling.
	/// </summary>
	public class PurchaseOperationsTests
	{
		[Fact]
		public async Task UpsertPurchaseAsync_Request_Writes_ExtId_Date_MessageId_And_Images()
		{
			// Arrange
			var handler = new FakeHandler(HttpStatusCode.OK, "{\"purchase\":{}}");
			var client = CreateClient(handler);
			var request = new UpsertPurchaseRequest("user@example.com", CreateItems(), messageId: "123.456")
			{
				ExtId = "4321",
				Date = new DateTimeOffset(2026, 9, 30, 19, 5, 7, TimeSpan.FromHours(1))
			};

			// Act
			var response = await client.Purchases.UpsertPurchaseAsync(request);

			// Assert
			Assert.True(response.IsSuccess);
			Assert.Equal(HttpMethod.Post, handler.Method);
			Assert.Equal("https://api.sailthru.com/purchase", handler.RequestUri!.GetLeftPart(UriPartial.Path));
			Assert.Equal("key", handler.Form!["api_key"]);
			Assert.Equal("json", handler.Form["format"]);

			var json = handler.Json!.Value;
			Assert.Equal("user@example.com", json.GetProperty("email").GetString());
			Assert.False(json.GetProperty("incomplete").GetBoolean());
			Assert.Equal("123.456", json.GetProperty("message_id").GetString());
			Assert.Equal("4321", json.GetProperty("purchase_keys").GetProperty("extid").GetString());
			Assert.Equal("2026-09-30T18:05:07+00:00", json.GetProperty("date").GetString());

			var item = json.GetProperty("items")[0];
			Assert.Equal("Booking-123", item.GetProperty("id").GetString());
			Assert.Equal("Spa - Package", item.GetProperty("title").GetString());
			Assert.Equal(12950, item.GetProperty("price").GetInt32());
			Assert.Equal(2, item.GetProperty("qty").GetInt32());
			Assert.Equal("https://example.com/spa/a/b/", item.GetProperty("url").GetString());
			Assert.Equal(JsonValueKind.Object, item.GetProperty("images").ValueKind);
			Assert.Equal("https://example.com/full.jpg", item.GetProperty("images").GetProperty("full").GetProperty("url").GetString());
			Assert.Equal("https://example.com/thumb.jpg", item.GetProperty("images").GetProperty("thumb").GetProperty("url").GetString());
			Assert.Equal("spa", item.GetProperty("tags")[0].GetString());
			Assert.Equal("true", item.GetProperty("vars").GetProperty("group_booking").GetString());
		}

		[Fact]
		public async Task UpsertPurchaseAsync_Legacy_Overload_Writes_Incomplete_And_No_ExtId_Or_Date()
		{
			// Arrange
			var handler = new FakeHandler(HttpStatusCode.OK, "{}");
			var client = CreateClient(handler);

			// Act
			var response = await client.Purchases.UpsertPurchaseAsync("user@example.com", CreateItems(), incomplete: true);

			// Assert
			Assert.True(response.IsSuccess);
			var json = handler.Json!.Value;
			Assert.True(json.GetProperty("incomplete").GetBoolean());
			Assert.Equal(JsonValueKind.Null, json.GetProperty("message_id").ValueKind);
			Assert.False(json.TryGetProperty("purchase_keys", out _));
			Assert.False(json.TryGetProperty("date", out _));
		}

		[Fact]
		public async Task UpsertPurchaseAsync_Writes_Order_Vars()
		{
			// Arrange
			var handler = new FakeHandler(HttpStatusCode.OK, "{}");
			var client = CreateClient(handler);
			var request = new UpsertPurchaseRequest("user@example.com", CreateItems())
			{
				Vars = new Map<string?> { ["order_ref"] = "4321" }
			};

			// Act
			await client.Purchases.UpsertPurchaseAsync(request);

			// Assert
			Assert.Equal("4321", handler.Json!.Value.GetProperty("vars").GetProperty("order_ref").GetString());
		}

		[Fact]
		public async Task UpsertPurchaseAsync_Incomplete_With_ExtId_Throws_And_Sends_Nothing()
		{
			// Arrange
			var handler = new FakeHandler(HttpStatusCode.OK, "{}");
			var client = CreateClient(handler);
			var request = new UpsertPurchaseRequest("user@example.com", CreateItems(), incomplete: true) { ExtId = "4321" };

			// Act / Assert
			await Assert.ThrowsAsync<ArgumentException>(() => client.Purchases.UpsertPurchaseAsync(request));
			Assert.Null(handler.Method);
		}

		[Fact]
		public async Task UpsertPurchaseAsync_Duplicate_ExtId_Is_Detected()
		{
			// Arrange
			var handler = new FakeHandler(HttpStatusCode.BadRequest, "{\"error\":99,\"errormsg\":\"Duplicate extid: 4321 ID already exists\"}");
			var client = CreateClient(handler);

			// Act
			var response = await client.Purchases.UpsertPurchaseAsync(
				new UpsertPurchaseRequest("user@example.com", CreateItems()) { ExtId = "4321" });

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.NotNull(response.Error);
			Assert.Equal(99, response.Error!.Code);
			Assert.Equal("Duplicate extid: 4321 ID already exists", response.Error.Message);
			Assert.True(response.Error.IsDuplicateExtId);
		}

		[Fact]
		public async Task UpsertPurchaseAsync_Other_Error_Is_Parsed()
		{
			// Arrange
			var handler = new FakeHandler(HttpStatusCode.BadRequest, "{\"error\":2,\"errormsg\":\"Invalid email: nope\"}");
			var client = CreateClient(handler);

			// Act
			var response = await client.Purchases.UpsertPurchaseAsync("nope", CreateItems());

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal(2, response.Error!.Code);
			Assert.Equal("Invalid email: nope", response.Error.Message);
			Assert.False(response.Error.IsDuplicateExtId);
			Assert.Equal("Sailthru error 2: Invalid email: nope", response.Error.ToString());
		}

		[Fact]
		public async Task UpsertPurchaseAsync_Non_Json_Error_Keeps_Status_Code()
		{
			// Arrange
			var handler = new FakeHandler(HttpStatusCode.BadGateway, "<html>Bad gateway</html>");
			var client = CreateClient(handler);

			// Act
			var response = await client.Purchases.UpsertPurchaseAsync("user@example.com", CreateItems());

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
			Assert.Null(response.Error!.Code);
			Assert.Null(response.Error.Exception);
		}

		[Fact]
		public async Task UpsertPurchaseAsync_Transport_Error_Is_A_Failed_Response()
		{
			// Arrange
			var client = new SailthruApiClient(new HttpClient(new ThrowingHandler()), CreateSettings());

			// Act
			var response = await client.Purchases.UpsertPurchaseAsync("user@example.com", CreateItems());

			// Assert
			Assert.False(response.IsSuccess);
			Assert.Equal((HttpStatusCode)0, response.StatusCode);
			Assert.Equal("Connection refused", response.Error!.Message);
			Assert.IsType<HttpRequestException>(response.Error.Exception);
		}

		[Fact]
		public async Task EnsureSuccess_Throws_SailthruException_With_Code()
		{
			// Arrange
			var handler = new FakeHandler(HttpStatusCode.BadRequest, "{\"error\":2,\"errormsg\":\"Invalid email: nope\"}");
			var client = CreateClient(handler);
			var response = await client.Purchases.UpsertPurchaseAsync("nope", CreateItems());

			// Act
			var exception = Assert.Throws<SailthruException>(() => response.EnsureSuccess());

			// Assert
			Assert.Equal(2, exception.Code);
			Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
			Assert.Equal("Sailthru error 2: Invalid email: nope", exception.Message);
		}

		[Fact]
		public void EnsureSuccess_Returns_Successful_Response()
		{
			// Arrange
			var response = new SailthruResponse(HttpMethod.Post, new Uri("https://api.sailthru.com/purchase"), true, HttpStatusCode.OK);

			// Act / Assert
			Assert.Same(response, response.EnsureSuccess());
		}

		[Theory]
		[InlineData("{\"message\":\"Legacy\",\"errors\":{\"email\":[\"Required\"]}}", null, "Legacy")]
		[InlineData("{\"error\":5}", 5, null)]
		[InlineData("[1,2]", null, null)]
		[InlineData("", null, null)]
		public void ParseError_Handles_Body_Shapes(string body, int? code, string? message)
		{
			// Act
			var error = ApiClient.ParseError(body);

			// Assert
			Assert.Equal(code, error.Code);
			Assert.False(string.IsNullOrEmpty(error.Message));
			if (message is not null)
			{
				Assert.Equal(message, error.Message);
			}
		}

		[Fact]
		public void ParseError_Reads_Field_Errors()
		{
			// Act
			var error = ApiClient.ParseError("{\"message\":\"Legacy\",\"errors\":{\"email\":[\"Required\"]}}");

			// Assert
			Assert.Equal(new[] { "Required" }, error.Errors!["email"]);
		}

		static SailthruSettings CreateSettings()
			=> new() { ApiKey = "key", ApiSecret = "secret" };

		static SailthruApiClient CreateClient(HttpMessageHandler handler)
			=> new(new HttpClient(handler), CreateSettings());

		static PurchaseItem[] CreateItems()
			=>
			[
				new PurchaseItem
				{
					Id = "Booking-123",
					Title = "Spa - Package",
					Price = 12950,
					Quantity = 2,
					Url = "https://example.com/spa/a/b/",
					Tags = ["spa"],
					Vars = new Map<string?> { ["group_booking"] = "true" },
					Images =
					[
						new PurchaseImage { Full = new PurchaseImageUrl { Url = "https://example.com/full.jpg" } },
						new PurchaseImage { Thumb = new PurchaseImageUrl { Url = "https://example.com/thumb.jpg" } }
					]
				}
			];

		class FakeHandler(HttpStatusCode status, string body) : HttpMessageHandler
		{
			public HttpMethod? Method { get; private set; }

			public Uri? RequestUri { get; private set; }

			public Dictionary<string, string>? Form { get; private set; }

			public JsonElement? Json { get; private set; }

			protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				Method = request.Method;
				RequestUri = request.RequestUri;

				var content = await request.Content!.ReadAsStringAsync(cancellationToken);
				Form = content.Split('&')
					.Select(p => p.Split('=', 2))
					.ToDictionary(p => Uri.UnescapeDataString(p[0]), p => Uri.UnescapeDataString(p[1].Replace('+', ' ')));
				Json = JsonDocument.Parse(Form["json"]).RootElement.Clone();

				return new HttpResponseMessage(status) { Content = new StringContent(body) };
			}
		}

		class ThrowingHandler : HttpMessageHandler
		{
			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
				=> throw new HttpRequestException("Connection refused");
		}
	}
}

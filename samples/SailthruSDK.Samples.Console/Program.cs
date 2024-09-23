namespace SailthruSDK.Samples.Console
{
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Threading.Tasks;

	using Microsoft.Extensions.Configuration;

	using SailthruSDK.Api;

	class Program
	{
		static async Task Main()
		{
			var settings = GetSettings();
			var http = GetHttpClient(settings);
			var client = new SailthruApiClient(http, settings);

			//var response = await client.Users.GetUserAsync(
			//	"me+spaseekers@matthewabbott.dev",
			//	key: UserKeyType.Email,
			//	fields: new UserFields
			//	{
			//		Activity = true,
			//		Engagement = true,
			//		Device = true,
			//		Keys = true,
			//		Lists = true,
			//		Lifetime = true,
			//		OptOutStatus = true,
			//		PurchaseIncomplete = 10,
			//		Purchases = 10,
			//		SmartLists = true,
			//		Vars = true
			//	});

			var response = await client.Users.UpsertUserAsync(
				"me@matthewabbott.dev",
				keys: new Map<string>
				{
					[UserKeyType.Email] = "me@matthewabbott.dev"
				},
				keyConflict: KeyConflict.Merge,
				cookies: new Map<string>
				{
					["sailthru_bid"] = "28309054.31001"
				});

			//var response = await client.Purchases.UpsertPurchaseAsync(
			//	"me+sailthru@matthewabbott.dev",
			//	[
			//		new PurchaseItem
			//		{
			//			Id = "MonetaryVoucher-50",
			//			Title = "£50.00 - Voucher",
			//			Price = 5000,
			//			Quantity = 1,
			//			Url = "https://www.spaseekers.com/spa-vouchers?value=50.00",
			//			Images = new[]
			//			{
			//				new PurchaseImage
			//				{
			//					Full = new PurchaseImageUrl
			//					{
			//						Url = "https://spaseekers.imgix.net/m/0/spaseekers-gift-vouchers-2022.jpg"
			//					}
			//				}
			//			}
			//		}
			//	],
			//	incomplete: true,
			//	messageId: "28309054.31001");
		}

		static SailthruSettings GetSettings()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("./appsettings.json", optional: false)
				.AddJsonFile("./appsettings.env.json", optional: true)
				.Build();

			var settings = new SailthruSettings();
			config.GetSection(SailthruSettings.ConfigurationSection).Bind(settings);

			settings.Validate();

			return settings;
		}
		static HttpClient GetHttpClient(SailthruSettings settings)
		{
			var http = new HttpClient()
			{
				BaseAddress = new System.Uri(settings.BaseUrl)
			};

			http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return http;
		}
	}
}

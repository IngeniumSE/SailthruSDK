namespace SailthruSDK.Samples.Console
{
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Threading.Tasks;

	using Microsoft.Extensions.Configuration;

	using SailthruSDK.Purchase;
	using SailthruSDK.User;

	class Program
	{
		static async Task Main()
		{
			var settings = GetSettings();
			var http = GetHttpClient(settings);
			var client = new SailthruClient(http, settings);

			//var user = await client.GetUserAsync(
			//	"me+spaseekers@matthewabbott.dev",
			//	fields: new SailthruUserFields
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
			//	}
			//);

			//var user = await client.UpsertUserAsync(
			//	"me@matthewabbott.dev",
			//	keys: new Map<string>
			//	{
			//		[SailthruUserKeyType.Email] = "me@matthewabbott.dev"
			//	},
			//	keyConflict: KeyConflict.Merge,
			//	cookies: new Map<string>
			//	{
			//		["sailthru_bid"] = "28309054.31001"
			//	});

			var response = await client.UpsertPurchaseAsync(
				"me+sailthru@matthewabbott.dev",
				new[] {
					new PurchaseItem("MonetaryVoucher-50", "£50.00 - Voucher", 5000, 1, "https://www.spaseekers.com/spa-vouchers?value=50.00")
				},
				incomplete: false,
				messageId: "28309054.31001"
				);
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

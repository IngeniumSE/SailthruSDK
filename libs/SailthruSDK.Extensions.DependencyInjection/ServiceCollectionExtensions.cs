namespace Microsoft.Extensions.DependencyInjection
{
	using System;
	using System.Net.Http;
	using System.Net.Http.Headers;

	using FluentValidation;

	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Options;

	using SailthruSDK;
	using SailthruSDK.Api;

	/// <summary>
	/// Provides extensions for the <see cref="IServiceCollection"/>
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Adds Sailthru services to the given services collection.
		/// </summary>
		/// <param name="services">The services collection.</param>
		/// <param name="configure">The configure delegate.</param>
		/// <returns>The services collection.</returns>
		public static IServiceCollection AddSailthru(
			this IServiceCollection services,
			Action<SailthruSettings> configure)
		{
			Ensure.IsNotNull(services, nameof(services));
			Ensure.IsNotNull(configure, nameof(configure));

			services.Configure(configure);

			AddCoreServices(services);

			return services;
		}

		/// <summary>
		/// Adds Sailthru services to the given services collection.
		/// </summary>
		/// <param name="services">The services collection.</param>
		/// <param name="settings">The Sailthru settings.</param>
		/// <returns>The services collection.</returns>
		public static IServiceCollection AddSailthru(
			this IServiceCollection services,
			SailthruSettings settings)
		{
			Ensure.IsNotNull(services, nameof(services));
			Ensure.IsNotNull(settings, nameof(settings));

			services.AddSingleton(settings.AsOptions());

			AddCoreServices(services);

			return services;
		}

		/// <summary>
		/// Adds Sailthru services to the given services collection.
		/// </summary>
		/// <param name="services">The services collection.</param>
		/// <param name="configuration">The configuration.</param>
		/// <returns>The services collection.</returns>
		public static IServiceCollection AddSailthru(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			Ensure.IsNotNull(services, nameof(services));
			Ensure.IsNotNull(configuration, nameof(configuration));

			services.Configure<SailthruSettings>(configuration.GetSection(SailthruSettings.ConfigurationSection));

			AddCoreServices(services);

			return services;
		}

		static void AddCoreServices(IServiceCollection services)
		{
			services.AddSingleton(sp =>
			{
				var settings = sp.GetRequiredService<IOptions<SailthruSettings>>().Value;

				settings.Validate();

				return settings;
			});

			services.AddScoped<ISailthruHttpClientFactory, SailthruHttpClientFactory>();
			services.AddScoped<ISailthruApiClientFactory, SailthruApiClientFactory>();

			AddApiClient(
				services,
				SailthruApiConstants.DefaultSailthruApiClient,
				(cf, settings) => cf.CreateApiClient(settings, SailthruApiConstants.DefaultSailthruApiClient));
		}

		static void AddApiClient<TClient>(
			IServiceCollection services,
			string name,
			Func<ISailthruApiClientFactory, SailthruSettings, TClient> factory)
			where TClient : class
		{
			void ConfigureHttpDefaults(HttpClient http)
			{
				http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			}

			services.AddHttpClient(name, ConfigureHttpDefaults);

			services.AddScoped(sp =>
			{
				var settings = sp.GetRequiredService<SailthruSettings>();
				var clientFactory = sp.GetRequiredService<ISailthruApiClientFactory>();

				return factory(clientFactory, settings);
			});
		}
	}
}

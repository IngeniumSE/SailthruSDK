namespace Microsoft.Extensions.DependencyInjection
{
	using System;
	using System.Net.Http;
	using System.Net.Http.Headers;

	using FluentValidation;

	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Options;

	using SailthruSDK;

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
			services.AddSingleton(sp => sp.GetRequiredService<IOptions<SailthruSettings>>().Value);

			AddCoreServices(services);

			return services;
		}

		static void AddCoreServices(IServiceCollection services)
		{
			services.AddHttpClient<SailthruClient>(
				"Sailthru",
				(sp, http) => ConfigureHttpClient(sp, http));
		}

		static void ConfigureHttpClient(IServiceProvider services, HttpClient http)
		{
			var settings = services.GetRequiredService<IOptions<SailthruSettings>>().Value;
			SailthruSettingsValidator.Instance.ValidateAndThrow(settings);

			http.BaseAddress = new Uri(settings.BaseUrl);
			http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}
	}
}
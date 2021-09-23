namespace SailthruSDK
{
	using System;
	using System.Net.Http;
	using System.Net.Http.Headers;

	using FluentValidation;

	using Microsoft.Extensions.DependencyInjection;

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
			IServiceCollection services,
			Action<SailthruSettings> configure)
		{
			Ensure.IsNotNull(services, nameof(services));
			Ensure.IsNotNull(configure, nameof(configure));

			services.Configure(configure);
			services.AddHttpClient<SailthruClient>(
				"Sailthru", 
				(sp, http) => ConfigureHttpClient(sp, http));

			return services;
		}

		static void ConfigureHttpClient(IServiceProvider services, HttpClient http)
		{
			var settings = services.GetRequiredService<SailthruSettings>();
			SailthruSettingsValidator.Instance.ValidateAndThrow(settings);

			http.BaseAddress = new Uri(settings.BaseUrl);
			http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}
	}
}
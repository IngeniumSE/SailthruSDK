namespace SailthruSDK
{
	using FluentValidation;

	/// <summary>
	/// Represents settings for configuring the Sailthru SDK.
	/// </summary>
	public class SailthruSettings
	{
		/// <summary>
		/// Gets or sets the API key.
		/// </summary>
		public string ApiKey { get; set; } = default!;

		/// <summary>
		/// Gets or sets the API secret.
		/// </summary>
		public string ApiSecret { get; set; } = default!;

		/// <summary>
		/// Gets or sets the base URL.
		/// </summary>
		public string BaseUrl { get; set; } = "https://api.sailthru.com";
	}

	/// <summary>
	/// Validates instances of <see cref="SailthruSettings"/>
	/// </summary>
	public class SailthruSettingsValidator : AbstractValidator<SailthruSettings>
	{
		public const string ConfigurationSection = "Sailthru";
		public static readonly SailthruSettingsValidator Instance = new SailthruSettingsValidator();

		public SailthruSettingsValidator()
		{
			RuleFor(r => r.ApiKey)
				.NotEmpty()
				.WithMessage("A Sailthru API key must be provided.");

			RuleFor(r => r.ApiSecret)
				.NotEmpty()
				.WithMessage("A Sailthru API secret must be provided.");

			RuleFor(r => r.BaseUrl)
				.NotEmpty()
				.WithMessage("A Sailthru URL must be provided.");
		}
	}
}

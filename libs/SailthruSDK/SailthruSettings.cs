namespace SailthruSDK
{
	using FluentValidation;

	using Microsoft.Extensions.Options;

	/// <summary>
	/// Represents settings for configuring the Sailthru SDK.
	/// </summary>
	public class SailthruSettings
	{
		public const string ConfigurationSection = "Sailthru";

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

		/// <summary>
		/// Gets or sets whether to capture request content.
		/// </summary>
		public bool CaptureRequestContent { get; set; }

		/// <summary>
		/// Gets or sets whether to capture response content.
		/// </summary>
		public bool CaptureResponseContent { get; set; }

		/// <summary>
		/// Returns the settings as an options instance.
		/// </summary>
		/// <returns>The options instance.</returns>
		public IOptions<SailthruSettings> AsOptions()
			=> Options.Create(this);

		/// <summary>
		/// Validates the current instance.
		/// </summary>
		public void Validate()
			=> new SailthruSettingsValidator().ValidateAndThrow(this);
	}

	/// <summary>
	/// Validates instances of <see cref="SailthruSettings"/>
	/// </summary>
	public class SailthruSettingsValidator : AbstractValidator<SailthruSettings>
	{
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

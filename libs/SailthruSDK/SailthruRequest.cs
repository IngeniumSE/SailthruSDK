namespace SailthruSDK
{
	using System.Net.Http;

	/// <summary>
	/// Represents a Sailthru request.
	/// </summary>
	/// <typeparam name="TRequest">The request type.</typeparam>
	public class SailthruRequest<TRequest>
		where TRequest : notnull
	{
		public SailthruRequest(
			HttpMethod method,
			string endpoint,
			TRequest model)
		{
			Method = method;
			Endpoint = Ensure.IsNotNullOrEmpty(endpoint, nameof(endpoint));
			Model = Ensure.IsNotNull(model, nameof(model));
		}

		/// <summary>
		/// Gets the Sailthru endpoint.
		/// </summary>
		public string Endpoint { get; }

		/// <summary>
		/// Gets the HTTP method.
		/// </summary>
		public HttpMethod Method { get; }

		/// <summary>
		/// Gets the model.
		/// </summary>
		public TRequest Model {  get; }
	}
}

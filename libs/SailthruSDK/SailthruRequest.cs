namespace SailthruSDK
{
	using System.Net.Http;

	/// <summary>
	/// Represents a Sailthru request.
	/// </summary>
	public class SailthruRequest<TModel>
		where TModel : notnull
	{
		public SailthruRequest(
			HttpMethod method,
			string endpoint,
			TModel model)
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
		public TModel Model {  get; }
	}
}

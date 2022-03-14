namespace SailthruSDK
{
	/// <summary>
	/// Represents a Sailthru response.
	/// </summary>
	/// <typeparam name="TResponse">The response type.</typeparam>
	internal class SailthruResponse<TResponse>
	{
		SailthruResponse(
			TResponse? result = default,
			bool isError = false)
		{
			IsError = isError;
			Result = result;
		}

		/// <summary>
		/// Gets whether the response was an error.
		/// </summary>
		public bool IsError { get; }

		/// <summary>
		/// Gets the Sailthru response result.
		/// </summary>
		public TResponse? Result {  get; }

		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <returns>The Sailthru response.</returns>
		public static SailthruResponse<TResponse> Success(TResponse? response)
			=> new SailthruResponse<TResponse>(response);

		/// <summary>
		/// Creates an error response.
		/// </summary>
		/// <returns>The Sailthru response.</returns>
		public static SailthruResponse<TResponse> Error()
			=> new SailthruResponse<TResponse>();
	}
}

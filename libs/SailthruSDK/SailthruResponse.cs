namespace SailthruSDK
{
	public class SailthruError
	{
		public SailthruError(
			int code,
			string message)
		{
			Code = code;
			Message = message;
		}

		/// <summary>
		/// Gets the error code.
		/// </summary>
		public int Code { get; }

		/// <summary>
		/// Gets the error message.
		/// </summary>
		public string Message { get; }
	}

	/// <summary>
	/// Represents a Sailthru response.
	/// </summary>
	public class SailthruResponse
	{
		protected SailthruResponse(
			bool isError = false,
			SailthruError? error = default)
		{
			IsError = isError;
			Error = error;
		}

		/// <summary>
		/// Gets whether the response was an error.
		/// </summary>
		public bool IsError { get; }

		/// <summary>
		/// Gets the error.
		/// </summary>
		public SailthruError? Error { get; }

		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <returns>The Sailthru response.</returns>
		public static SailthruResponse Success()
			=> new SailthruResponse(isError: false);

		/// <summary>
		/// Creates an error response.
		/// </summary>
		/// <returns>The Sailthru response.</returns>
		public static SailthruResponse Failure(SailthruError error)
			=> new SailthruResponse(isError: true, error: error);
	}

	/// <summary>
	/// Represents a Sailthru response.
	/// </summary>
	/// <typeparam name="TResponse">The response type.</typeparam>
	public class SailthruResponse<TResponse> : SailthruResponse
	{
		SailthruResponse(
			TResponse? result = default,
			bool isError = false,
			SailthruError? error = default)
			: base(isError, error)
		{
			Result = result;
		}

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
		public static new SailthruResponse<TResponse> Failure(SailthruError error)
			=> new SailthruResponse<TResponse>(isError: true, error: error);
	}
}

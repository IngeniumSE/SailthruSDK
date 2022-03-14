namespace SailthruSDK
{
	/// <summary>
	/// Represents a value that can be one of the following types.
	/// </summary>
	/// <typeparam name="TFirst">The first type.</typeparam>
	/// <typeparam name="TSecond">The second type.</typeparam>
	public struct OneOf<TFirst, TSecond>
	{
		/// <summary>
		/// Creates an instance of <see cref="OneOf{TFirst, TSecond}"/> representing the first type.
		/// </summary>
		/// <param name="first">The first value.</param>
		public OneOf(TFirst first)
		{
			First = first;
			Second = default!;
			HasValue = true;
			IsFirst = true;
		}

		/// <summary>
		/// Creates an instance of <see cref="OneOf{TFirst, TSecond}"/> representing the second type.
		/// </summary>
		/// <param name="second">The second value.</param>
		public OneOf(TSecond second)
		{
			First = default!;
			Second = second;
			HasValue = true;
			IsFirst = false;
		}

		/// <summary>
		/// Gets whether we have a value.
		/// </summary>
		public bool HasValue { get; }

		/// <summary>
		/// Gets whether the value is the first value.
		/// </summary>
		public bool IsFirst { get; }

		/// <summary>
		/// Gets the first value.
		/// </summary>
		public TFirst First { get; }

		/// <summary>
		/// Gets the second value.
		/// </summary>
		public TSecond Second { get; }
	}
}

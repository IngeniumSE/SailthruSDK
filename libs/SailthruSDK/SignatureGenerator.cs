namespace SailthruSDK
{
	using System.Security.Cryptography;
	using System.Text;

	/// <summary>
	/// Provides services for generating signatures.
	/// </summary>
	public static class SignatureGenerator
	{
		/// <summary>
		/// Generates a HMAC signature for the given values.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="secret">The secret.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>The</returns>
		public static string Generate(
			string key,
			string secret,
			string json)
		{
			Ensure.IsNotNullOrEmpty(key, nameof(key));
			Ensure.IsNotNullOrEmpty(secret, nameof(secret));

			var builder = new StringBuilder();
			builder.Append(key);
			builder.Append(secret);
			builder.Append("json");

			if (json is { Length: > 0 })
			{
				builder.Append(json);
			}

			var bytes = Encoding.UTF8.GetBytes(builder.ToString());
			using var mdf = MD5.Create();

			var hashed = mdf.ComputeHash(bytes);
			var output = new StringBuilder();
			for (int i =0; i < hashed.Length; i++)
			{
				output.Append(hashed[i].ToString("x2"));
			}

			return output.ToString();
		}
	}
}
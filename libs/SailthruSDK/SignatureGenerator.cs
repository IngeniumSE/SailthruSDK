namespace SailthruSDK
{
	using System;
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
		/// <param name="format">The request format.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>The signature.</returns>
		public static string Generate(
			string key,
			string secret,
			string format = "json",
			string? payload = default)
		{
			Ensure.IsNotNullOrEmpty(key, nameof(key));
			Ensure.IsNotNullOrEmpty(secret, nameof(secret));
			Ensure.IsNotNullOrEmpty(format, nameof(format));

			/*
			 * Sailthru documentation is slightly confusing:
			 * https://getstarted.sailthru.com/developers/api-basics/technical/
			 * 
			 * This is not as clear as the documentation suggests.
			 * - The payload values must contain the API key and format (e.g. 'json') before calculating the signature
			 * - The api secret is combined with the payload values (in a concatenated, sorted set) to generate the signature
			 * 
			 * E.g.
			 * - API key: 123key
			 * - API secret: abcsecret
			 * - Format: json
			 * - Payload: {"id":"neil@example.com"}
			 * 
			 * - Payload values = [123key, json, {"id":"neil@example.com"}] - sorted, and concatenated
			 * - Signature = secret + payload values e.g. abcsecret123keyjson{"id":"neil@example.com"} - as an MD5 hash
			 */

			var builder = new StringBuilder();
			builder.Append(secret);
			builder.Append(GetSignaturePayload(key, format, payload));

			var bytes = Encoding.UTF8.GetBytes(builder.ToString());
			using var mdf = MD5.Create();

			var hashed = mdf.ComputeHash(bytes);
			var output = new StringBuilder();
			for (int i =0; i < hashed.Length; i++)
			{
				output.Append(hashed[i].ToString("x2"));
			}

			// Sailthru documentation requests signature values are generated with lowercase letters.
			return output.ToString().ToLower();
		}

		static string GetSignaturePayload(string key, string format, string? payload)
		{
			string[] values = payload is { Length: > 0 } ? new string[3] : new string[2];
			values[0] = key;
			values[1] = format;

			if (payload is { Length: > 0 })
			{
				values[2] = payload;
			}

			Array.Sort(values, StringComparer.Ordinal);

			return string.Join("", values);
		}
	}
}
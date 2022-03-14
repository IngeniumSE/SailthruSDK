namespace SailthruSDK.Tests
{
	using Xunit;

	/// <summary>
	/// Provides tests for the <see cref="SignatureGenerator"/> type.
	/// </summary>
	public class SignatureGeneratorTests
	{
		[Fact]
		void Generate_Generates_Correct_Signature()
		{
			// Arrange
			string expected = "fa5c79189b708199f3cf69f1cf8f7928";

			// Act
			string result = SignatureGenerator.Generate(
				"123key",
				"abcsecret",
				"json",
				"{\"id\":\"neil@example.com\"}");

			// Assert
			Assert.Equal(expected, result);
		}
	}
}
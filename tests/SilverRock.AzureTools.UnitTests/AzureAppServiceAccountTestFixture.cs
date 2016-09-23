using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SilverRock.AzureTools.UnitTests
{
	[TestClass]
	public class AzureAppServiceAccountTestFixture
	{
		[TestMethod]
		public void TestGetEndpoint()
		{
			// Arrange
			string name = "3af03971-f155-4d6e-aff6-c16cbfa52dc0";
			string expected = $"https://{name}.scm.azurewebsites.net/";

			// Act
			string result = AzureAppServiceAccount.GetEndpoint(name);

			// Assert
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void TestGetAuthHeader()
		{
			// Arrange
			string username = "AzureDiamond";
			string password = "hunter2";
			string expected = $"Basic QXp1cmVEaWFtb25kOmh1bnRlcjI=";

			// Act
			string result = AzureAppServiceAccount.GetAuthHeader(username, password);

			// Assert
			Assert.AreEqual(expected, result);
		}
	}
}

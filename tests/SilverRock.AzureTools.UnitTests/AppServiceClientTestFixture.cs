using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace SilverRock.AzureTools.UnitTests
{
	[TestClass]
	public class AppServiceClientTestFixture
	{
		[TestMethod]
		public void TestGetSettings()
		{
			// Arrange
			var account = new Mock<AppServiceAccount>(string.Empty, string.Empty, string.Empty);

			const string key1 = "key1";
			const string value1 = "value1";
			const string key2 = "key2";
			const string value2 = "value2";

			Dictionary<string, string> expected = new Dictionary<string, string> { { key1, value1 }, { key2, value2 } };

			account
				.Setup(c => c.GetResource(It.Is<string>(s => s == AppServiceClient.SETTINGS)))
				.Returns($"{{ \"{key1}\": \"{value1}\", \"{key2}\": \"{value2}\" }}");

			AppServiceClient sut = new AppServiceClient(account.Object);

			// Act
			var result = sut.GetSettings();

			// Assert
			CollectionAssert.AreEquivalent(expected, result);
		}

		[TestMethod]
		public void TestGetSetting()
		{
			// Arrange
			var account = new Mock<AppServiceAccount>(string.Empty, string.Empty, string.Empty);

			string key = "key";
			string value = "value";

			string expected = value;

			account
				.Setup(c => c.GetResource(It.Is<string>(s => s == $"{AppServiceClient.SETTINGS}/{key}")))
				.Returns($"\"{value}\"");

			AppServiceClient sut = new AppServiceClient(account.Object);

			// Act
			var result = sut.GetSetting(key);

			// Assert
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void TestToJson()
		{
			// Arrange
			const string key1 = nameof(key1);
			const string value1 = nameof(value1);
			const string key2 = nameof(key2);
			const string value2 = nameof(value2);

			Dictionary<string, string> data = new Dictionary<string, string> { { key1, value1 }, { key2, value2 } };

			// Act
			var result = AppServiceClient.ToJson(data);

			// Assert
			Assert.IsTrue(result.Contains(key1));
			Assert.IsTrue(result.Contains(value1));
			Assert.IsTrue(result.Contains(key2));
			Assert.IsTrue(result.Contains(value2));
		}

		[TestMethod]
		public void TestToDictionary()
		{
			// Arrange
			const string key1 = nameof(key1);
			const string value1 = nameof(value1);
			const string key2 = nameof(key2);
			const string value2 = nameof(value2);

			string json = $"{{ \"{key1}\": \"{value1}\", \"{key2}\": \"{value2}\" }}";
			Dictionary<string, string> expected = new Dictionary<string, string> { { key1, value1 }, { key2, value2 } };

			// Act
			var result = AppServiceClient.ToDictionary(json);

			// Assert
			CollectionAssert.AreEquivalent(expected, result);
		}
	}
}

using Microsoft.ServiceBus.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SilverRock.AzureTools.Models;

namespace SilverRock.AzureTools.UnitTests
{
	[TestClass]
	public class ScriptRunnerTestFixture
	{

		[TestCategory(nameof(ScriptRunner))]
		[TestMethod]
		public void WhenTopicExistsAndCreateIsNotForced()
		{
			DoTest(
				topicExists: true,
				createIsForced: false,
				deleteTopicCalled: Times.Never(),
				createTopicCalled: Times.Never(),
				createSubCalled: Times.Never());
		}

		[TestCategory(nameof(ScriptRunner))]
		[TestMethod]
		public void WhenTopicExistsAndCreateIsForced()
		{
			DoTest(
				topicExists: true,
				createIsForced: true,
				deleteTopicCalled: Times.Once(),
				createTopicCalled: Times.Once(),
				createSubCalled: Times.Exactly(2));
		}

		[TestCategory(nameof(ScriptRunner))]
		[TestMethod]
		public void WhenTopicDoesNotExistAndCreateIsForced()
		{
			DoTest(
				topicExists: false,
				createIsForced: true,
				deleteTopicCalled: Times.Never(),
				createTopicCalled: Times.Once(),
				createSubCalled: Times.Exactly(2));
		}

		[TestCategory(nameof(ScriptRunner))]
		[TestMethod]
		public void WhenTopicDoesNotExistAndCreateIsNotForced()
		{
			DoTest(
				topicExists: false,
				createIsForced: false,
				deleteTopicCalled: Times.Never(),
				createTopicCalled: Times.Once(),
				createSubCalled: Times.Exactly(2));
		}

		private void DoTest(bool topicExists, bool createIsForced, Times deleteTopicCalled, Times createTopicCalled, Times createSubCalled)
		{
			// Arrange
			var stub = new Mock<INamespaceService>();

			stub.Setup(ns => ns.TopicExists(It.IsAny<string>())).Returns(topicExists);

			string script = $@"
{{
	topics: [
		{{
			path: 'some-topic',
			subscriptions: [
				{{ name: 'some-subscription' }},
				{{ name: 'another-subscription' }}
			]
		}}
	]
}}";

			ScriptRunner sut = new ScriptRunner(stub.Object);

			// Act
			sut.Create(script, force: createIsForced);

			// Assert
			stub.Verify(ns => ns.DeleteTopic(It.IsAny<string>()), deleteTopicCalled);
			stub.Verify(ns => ns.CreateTopic(It.IsAny<TopicDescription>()), createTopicCalled);
			stub.Verify(ns => ns.CreateSubscription(It.IsAny<SubscriptionDescription>()), createSubCalled);
		}

		[TestCategory(nameof(ScriptRunner))]
		[TestMethod]
		public void ParseScriptTest()
		{
			// Arrange
			string path = "my-topic";

			string script = $@"
{{
	topics: [
		{{
			path: '{path}',
			authorization: [
				{{ name: 'default', accessRights: ['Manage', 'Send', 'Listen'] }}
			],
			subscriptions: [
				{{ name: 'some-subscription' }},
				{{ name: 'another-subscription' }}
			]
		}}
	]
}}";

			ScriptRunner sut = new ScriptRunner(default(INamespaceService));

			// Act
			Script result = sut.ParseScript(script);

			// Assert
			Assert.IsNotNull(result);
		}
	}
}

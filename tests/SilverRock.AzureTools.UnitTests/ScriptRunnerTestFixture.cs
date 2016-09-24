using Microsoft.ServiceBus.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SilverRock.AzureTools.Models;
using SilverRock.AzureTools.Models.ServiceBus;
using SilverRock.AzureTools.Models.ServicesBus;
using System.Collections.Generic;

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

			var namespaceService = new Mock<INamespaceService>();
			namespaceService
				.Setup(ns => ns.TopicExists(It.IsAny<string>()))
				.Returns(topicExists);

			var serviceLocator = new Mock<IServiceLocator>();
			serviceLocator
				.Setup(s => s.GetNamespaceService(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(namespaceService.Object);

			Script script = new Script
			{
				DeployEnvironments = new List<DeployEnvironment>
				{
					new DeployEnvironment
					{
						Name = "test",
						Topics = new Topics
						{
							Create = new List<Topic>
							{
								new Topic
								{
									Namespace = new Namespace
									{
										Endpoint = "asdf",
										AccessKey = "asdf",
										AccessKeyName = "asdf"
									},
									Path = "some-topic",
									Subscriptions = new List<Subscription>
									{
										new Subscription { Name = "some-subscription" },
										new Subscription { Name = "another-subscription" }
									}
								}
							}
						}
					}
				}
			};

			ScriptRunner sut = new ScriptRunner(serviceLocator.Object);

			// Act
			sut.Run(script, force: createIsForced);

			// Assert
			namespaceService.Verify(ns => ns.DeleteTopic(It.IsAny<string>()), deleteTopicCalled);
			namespaceService.Verify(ns => ns.CreateTopic(It.IsAny<TopicDescription>()), createTopicCalled);
			namespaceService.Verify(ns => ns.CreateSubscription(It.IsAny<SubscriptionDescription>()), createSubCalled);
		}
	}
}

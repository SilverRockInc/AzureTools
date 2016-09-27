using Microsoft.ServiceBus.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SilverRock.AzureTools.Models;
using SilverRock.AzureTools.Models.AppService;
using SilverRock.AzureTools.Models.ServiceBus;
using SilverRock.AzureTools.Models.ServicesBus;
using System;
using System.Collections.Generic;
using System.Linq;

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

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void UpdateAppServiceWhenAppServicesIsNull()
		{
			// Arrange
			ScriptRunner sut = new ScriptRunner(new Mock<IServiceLocator>().Object);
			AppService appService = null;

			// Act
			sut.UpdateAppService(appService);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateAppServiceWhenAccountsIsNull()
		{
			// Arrange
			ScriptRunner sut = new ScriptRunner(new Mock<IServiceLocator>().Object);
			AppService appService = new AppService();

			// Act
			sut.UpdateAppService(appService);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateAppServiceWhenAccountsIsEmpty()
		{
			// Arrange
			ScriptRunner sut = new ScriptRunner(new Mock<IServiceLocator>().Object);
			AppService appService = new AppService { Accounts = new List<Account>() };

			// Act
			sut.UpdateAppService(appService);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateAppServiceWhenServiceNameIsEmpty()
		{
			// Arrange
			ScriptRunner sut = new ScriptRunner(new Mock<IServiceLocator>().Object);
			AppService appService = new AppService
			{
				Accounts = new List<Account>
				{
					new Account { Username = "asdf" , Password = "asdf" }
				}
			};

			// Act
			sut.UpdateAppService(appService);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateAppServiceWhenUsernameIsEmpty()
		{
			// Arrange
			ScriptRunner sut = new ScriptRunner(new Mock<IServiceLocator>().Object);
			AppService appService = new AppService
			{
				Accounts = new List<Account>
				{
					new Account { ServiceName = "asdf" , Password = "asdf" }
				}
			};

			// Act
			sut.UpdateAppService(appService);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateAppServiceWhenPasswordIsEmpty()
		{
			// Arrange
			ScriptRunner sut = new ScriptRunner(new Mock<IServiceLocator>().Object);
			AppService appService = new AppService
			{
				Accounts = new List<Account>
				{
					new Account { ServiceName = "asdf" , Username = "asdf" }
				}
			};

			// Act
			sut.UpdateAppService(appService);
		}

		[TestMethod]
		public void OnlyCorrectEnvironmentIsRun()
		{
			// Arrange
			var namespaceService = new Mock<INamespaceService>();
			namespaceService
				.Setup(ns => ns.TopicExists(It.IsAny<string>()))
				.Returns(false);

			var serviceLocator = new Mock<IServiceLocator>();
			serviceLocator
				.Setup(s => s.GetNamespaceService(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(namespaceService.Object);

			Topic topic = new Topic
			{
				Namespace = new Namespace
				{
					Endpoint = "asdf",
					AccessKey = "asdf",
					AccessKeyName = "asdf"
				},
				Path = "asdf"
			};

			string env1Name = nameof(env1Name);
			string env2Name = nameof(env2Name);

			List<Topic> env1Topics = new List<Topic> { topic, topic };
			List<Topic> env2Topics = new List<Topic> { topic, topic, topic, topic, topic };

			Script script = new Script
			{
				DeployEnvironments = new List<DeployEnvironment>
				{
					new DeployEnvironment
					{
						Name = env1Name,
						Topics = new Topics { Create = env1Topics }
					},
					new DeployEnvironment
					{
						Name = env1Name,
						Topics = new Topics { Create = env1Topics }
					},
					new DeployEnvironment
					{
						Name = env2Name,
						Topics = new Topics { Create = env2Topics }
					}
				}
			};

			int expectedEnv1TopicCount = script.DeployEnvironments.Where(e => e.Name == env1Name).SelectMany(e => e.Topics.Create).Count();
			int expectedEnv2TopicCount = script.DeployEnvironments.Where(e => e.Name == env2Name).SelectMany(e => e.Topics.Create).Count();

			ScriptRunner sut = new ScriptRunner(serviceLocator.Object);

			// Act
			sut.Run(script, env1Name);

			// Assert
			Assert.AreNotEqual(expectedEnv1TopicCount, expectedEnv2TopicCount);
			namespaceService.Verify(ns => ns.CreateTopic(It.IsAny<TopicDescription>()), Times.Exactly(expectedEnv1TopicCount));
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

			string environment = "test";

			Script script = new Script
			{
				DeployEnvironments = new List<DeployEnvironment>
				{
					new DeployEnvironment
					{
						Name = environment,
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
			sut.Run(script, environment, force: createIsForced);

			// Assert
			namespaceService.Verify(ns => ns.DeleteTopic(It.IsAny<string>()), deleteTopicCalled);
			namespaceService.Verify(ns => ns.CreateTopic(It.IsAny<TopicDescription>()), createTopicCalled);
			namespaceService.Verify(ns => ns.CreateSubscription(It.IsAny<SubscriptionDescription>()), createSubCalled);
		}
	}
}

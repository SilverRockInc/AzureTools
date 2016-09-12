using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace SilverRock.AzureTools.Services
{
	/// <summary>
	/// Facade for Microsoft.ServiceBus.NamespaceManager
	/// </summary>
	internal sealed class AzureNamespaceService : INamespaceService
	{
		public AzureNamespaceService(NamespaceManager namespaceManager)
		{
			_namespaceManager = namespaceManager;
		}

		public bool TopicExists(string path)
		{
			return _namespaceManager.TopicExists(path);
		}

		public void DeleteTopic(string path)
		{
			_namespaceManager.DeleteTopic(path);
		}

		public void CreateTopic(TopicDescription description)
		{
			_namespaceManager.CreateTopic(description);
		}

		public void CreateSubscription(SubscriptionDescription description)
		{
			_namespaceManager.CreateSubscription(description);
		}

		public void CreateSubscription(SubscriptionDescription description, Filter filter)
		{
			_namespaceManager.CreateSubscription(description, filter);
		}

		readonly NamespaceManager _namespaceManager;
	}
}

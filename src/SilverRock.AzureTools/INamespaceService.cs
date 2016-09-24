using Microsoft.ServiceBus.Messaging;

namespace SilverRock.AzureTools
{
	/// <summary>
	/// Mockable facade for Microsoft.ServiceBus.NamespaceManager
	/// </summary>
	public interface INamespaceService
	{
		bool TopicExists(string path);
		void DeleteTopic(string path);
		void CreateTopic(TopicDescription description);
		void CreateSubscription(SubscriptionDescription description);
		void CreateSubscription(SubscriptionDescription description, Filter filter);
	}
}

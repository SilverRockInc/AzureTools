using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using SilverRock.AzureTools.Models;
using SilverRock.AzureTools.Services;
using System;
using System.Linq;

namespace SilverRock.AzureTools
{
	public class ScriptRunner
	{
		public ScriptRunner(NamespaceManager namespaceManager)
		{
			_namespaceService = new AzureNamespaceService(namespaceManager);
		}

		public ScriptRunner(string endpoint, string accessKeyName, string accessKey)
		{
			_namespaceService = new AzureNamespaceService(NamespaceManager.CreateFromConnectionString(CreateConnectionString(endpoint, accessKeyName, accessKey)));
		}

		internal ScriptRunner(INamespaceService namespaceService)
		{
			_namespaceService = namespaceService;
		}

		public void Create(string script, bool force = false)
		{
			Script s = ParseScript(script);

			OnMessage($"Creating {s.Topics.Count} topics ... " + Environment.NewLine + Environment.NewLine);

			foreach (Topic topic in s.Topics ?? Enumerable.Empty<Topic>())
			{
				if (_namespaceService.TopicExists(topic.Path))
				{
					if (force)
					{
						OnMessage($"Topic '{topic.Path}' already exists.  Deleting ... ");
						_namespaceService.DeleteTopic(topic.Path);
						OnMessage("done" + Environment.NewLine);
					}
					else
					{
						OnMessage($"Topic '{topic.Path}' already exists.  Skipping ... " + Environment.NewLine);
						continue;
					}
				}

				TopicDescription topicDescription = Mappers.TopicMapper.Map(topic);

				OnMessage($"Creating topic '{topic.Path}' ... ");
				_namespaceService.CreateTopic(topicDescription);
				OnMessage("done" + Environment.NewLine);

				foreach (Subscription subscription in topic.Subscriptions ?? Enumerable.Empty<Subscription>())
				{
					SubscriptionDescription subscriptionDescription = Mappers.SubscriberMapper.Map(subscription, topic.Path);

					OnMessage($"Creating subscription '{subscription.Name}' for topic '{topic.Path}' ... ");
					if (subscription.SqlFilter == null)
						_namespaceService.CreateSubscription(subscriptionDescription);
					else
						_namespaceService.CreateSubscription(subscriptionDescription, new SqlFilter(subscription.SqlFilter));
					OnMessage("done" + Environment.NewLine);
				}
			}
		}

		public Script ParseScript(string script)
		{
			return JsonConvert.DeserializeObject<Script>(script);
		}

		internal static string CreateConnectionString(string endpoint, string accessKeyName, string accessKey)
		{
			return $"Endpoint={endpoint};SharedAccessKeyName={accessKeyName};SharedAccessKey={accessKey}";
		}

		public event EventHandler<MessageEventArgs> Message;

		protected virtual void OnMessage(string message)
		{
			Message?.Invoke(this, new MessageEventArgs(message));
		}

		readonly INamespaceService _namespaceService;
	}

	public class MessageEventArgs : EventArgs
	{
		public MessageEventArgs(string message)
		{
			Message = message;
		}

		public string Message { get; }
	}
}
